using Microsoft.EntityFrameworkCore;
namespace TravelFriend;

public class LiquidacionService : ILiquidacionService
{
    private readonly AppDbContext _context;

    public LiquidacionService(AppDbContext context)
    {
        _context = context;
    }


    public List<Liquidacion> CalcularLiquidaciones(Viaje viaje)
    {
        var balanceMap = new Dictionary<int, decimal>();

        foreach (var gasto in viaje.Gastos)
        {
            if (!balanceMap.ContainsKey(gasto.UsuarioId)) balanceMap[gasto.UsuarioId] = 0;
            balanceMap[gasto.UsuarioId] += gasto.Monto;

            foreach (var detalle in gasto.Detalles)
            {
                decimal montoConsumido = Math.Round(gasto.Monto * (detalle.Porcentaje / 100m), 2);

                if (!balanceMap.ContainsKey(detalle.UsuarioId)) balanceMap[detalle.UsuarioId] = 0;
                balanceMap[detalle.UsuarioId] -= montoConsumido;
            }
        }

        var deudores = balanceMap
            .Where(b => b.Value < -0.01m) // Tolerancia de un centavo
            .Select(b => new BalanceUsuarioDto { UsuarioId = b.Key, Monto = Math.Abs(b.Value) })
            .OrderByDescending(b => b.Monto) 
            .ToList();

        var acreedores = balanceMap
            .Where(b => b.Value > 0.01m)
            .Select(b => new BalanceUsuarioDto { UsuarioId = b.Key, Monto = b.Value })
            .OrderByDescending(b => b.Monto)
            .ToList();

        var liquidaciones = new List<Liquidacion>();
        int i = 0; // Índice para deudores
        int j = 0; // Índice para acreedores

        while (i < deudores.Count && j < acreedores.Count)
        {
            var deudor = deudores[i];
            var acreedor = acreedores[j];

            decimal montoTransaccion = Math.Min(deudor.Monto, acreedor.Monto);

            if (montoTransaccion > 0.01m)
            {
                liquidaciones.Add(new Liquidacion
                {
                    ViajeId = viaje.Id,
                    DeudorId = deudor.UsuarioId,
                    AcreedorId = acreedor.UsuarioId,
                    Monto = montoTransaccion,
                    Pagado = false
                });
            }

            deudor.Monto -= montoTransaccion;
            acreedor.Monto -= montoTransaccion;

            if (deudor.Monto < 0.01m) i++;
            if (acreedor.Monto < 0.01m) j++;
        }

        return liquidaciones;
    }


    public async Task<List<LiquidacionResumenDto>> ObtenerLiquidacionesResumenAsync(int viajeId)
    {
        return await _context.Liquidaciones
            .Where(l => l.ViajeId == viajeId)
            .Select(l => new LiquidacionResumenDto
            {
                Id = l.Id,
                Monto = l.Monto,
                Pagado = l.Pagado,
                Deudor = new UsuarioMinimoDto
                {
                    Id = l.Deudor.Id,
                    Nombre = l.Deudor.Nombre,
                    Avatar = l.Deudor.AvatarIniciales
                },
                Acreedor = new UsuarioMinimoDto
                {
                    Id = l.Acreedor.Id,
                    Nombre = l.Acreedor.Nombre,
                    Avatar = l.Acreedor.AvatarIniciales
                }
            })
            .ToListAsync();
    }

    public async Task<List<BalanceUsuarioDto>> ObtenerBalanceEnVivoAsync(int viajeId)
    {
        var viaje = await _context.Viajes
            .Include(v => v.Participantes)
            .Include(v => v.Gastos).ThenInclude(g => g.Detalles)
            .FirstOrDefaultAsync(v => v.Id == viajeId);

        if (viaje == null) return new List<BalanceUsuarioDto>();

        var balanceMap = new Dictionary<int, decimal>();

        foreach (var p in viaje.Participantes) balanceMap[p.UsuarioId] = 0;

        foreach (var gasto in viaje.Gastos)
        {
            if (!balanceMap.ContainsKey(gasto.UsuarioId)) balanceMap[gasto.UsuarioId] = 0;
            balanceMap[gasto.UsuarioId] += gasto.Monto;

            foreach (var d in gasto.Detalles)
            {
                decimal consumo = Math.Round(gasto.Monto * (d.Porcentaje / 100m), 2);
                if (!balanceMap.ContainsKey(d.UsuarioId)) balanceMap[d.UsuarioId] = 0;
                balanceMap[d.UsuarioId] -= consumo;
            }
        }

        return balanceMap.Select(kv => new BalanceUsuarioDto
        {
            UsuarioId = kv.Key,
            Monto = kv.Value
        }).ToList();
    }


    public async Task<bool> MarcarDeudaPagadaAsync(int liquidacionId, int usuarioId)
    {
        var deuda = await _context.Liquidaciones.FindAsync(liquidacionId);
        if (deuda == null) return false;

        if (deuda.DeudorId != usuarioId && deuda.AcreedorId != usuarioId)
        {
            throw new UnauthorizedAccessException("No tienes permiso para gestionar esta deuda.");
        }

        deuda.Pagado = true;
        // deuda.FechaPago = DateTime.Now; 

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ResumenGastosDto>> ObtenerResumenGastosAsync(int viajeId)
    {
        var viaje = await _context.Viajes
            .Include(v => v.Participantes).ThenInclude(p => p.Usuario)
            .Include(v => v.Gastos).ThenInclude(g => g.Detalles)
            .FirstOrDefaultAsync(v => v.Id == viajeId);

        if (viaje == null) return new List<ResumenGastosDto>();

        var resumenMap = viaje.Participantes.ToDictionary(
            p => p.UsuarioId,
            p => new ResumenGastosDto
            {
                UsuarioId = p.UsuarioId,
                Nombre = p.Usuario.Nombre,
                TotalPagado = 0,
                TotalConsumido = 0
            });

        foreach (var gasto in viaje.Gastos)
        {
            if (resumenMap.ContainsKey(gasto.UsuarioId))
            {
                resumenMap[gasto.UsuarioId].TotalPagado += gasto.Monto;
            }

            foreach (var detalle in gasto.Detalles)
            {
                if (resumenMap.ContainsKey(detalle.UsuarioId))
                {
                    decimal consumo = gasto.Monto * (detalle.Porcentaje / 100m);
                    resumenMap[detalle.UsuarioId].TotalConsumido += consumo;
                }
            }
        }

        var resultado = resumenMap.Values.ToList();
        foreach (var r in resultado)
        {
            r.TotalPagado = Math.Round(r.TotalPagado, 2);
            r.TotalConsumido = Math.Round(r.TotalConsumido, 2);
        }

        return resultado.OrderByDescending(x => x.TotalPagado).ToList();
    }
}
