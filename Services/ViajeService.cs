using Microsoft.EntityFrameworkCore;

namespace TravelFriend;

public class ViajeService : IViajeService
{
    private readonly AppDbContext _context;
    private readonly ILiquidacionService _liquidacionService;

    public ViajeService(AppDbContext context, ILiquidacionService liquidacionService)
    {
        _context = context;
        _liquidacionService = liquidacionService;
    }


    public async Task<Viaje> CrearViajeAsync(Viaje viaje, List<int> participantesIds)
    {
        // Buscamos en la BD si los usuarios invitados son realmente amigos del creador
        var amigosReales = await _context.Amigos
            .Where(a => (a.UsuarioId == viaje.CreadorId && participantesIds.Contains(a.AmigoId)) ||
                        (a.AmigoId == viaje.CreadorId && participantesIds.Contains(a.UsuarioId)))
            .Select(a => a.UsuarioId == viaje.CreadorId ? a.AmigoId : a.UsuarioId)
            .ToListAsync();

        var infiltrados = participantesIds.Except(amigosReales).ToList();

        if (infiltrados.Any())
        {
            throw new ArgumentException($"No puedes crear el viaje. Los usuarios con ID [{string.Join(", ", infiltrados)}] no son tus amigos.");
        }

        viaje.Estado = "Activo";
        viaje.FechaInicio = DateTime.Now;

        _context.Viajes.Add(viaje);
        await _context.SaveChangesAsync(); 

     
        _context.ParticipanteViajes.Add(new ParticipanteViaje 
        { 
            UsuarioId = viaje.CreadorId, 
            ViajeId = viaje.Id, 
            Presupuesto = 0 
        });

        foreach (var uid in participantesIds)
        {
            // Evitamos duplicar al creador si vino en la lista
            if (uid != viaje.CreadorId)
            {
                _context.ParticipanteViajes.Add(new ParticipanteViaje 
                { 
                    UsuarioId = uid, 
                    ViajeId = viaje.Id, 
                    Presupuesto = 0 
                });
            }
        }

        await _context.SaveChangesAsync();
        return viaje;
    }

  
    public async Task<ViajeResumenDto?> ObtenerViajeAsync(int viajeId)
    {
        return await _context.Viajes
            .Where(v => v.Id == viajeId)
            .Select(v => new ViajeResumenDto
            {
                Id = v.Id,
                Nombre = v.Nombre,
                Ubicacion = v.Ubicacion,
                FechaInicio = v.FechaInicio,
                Estado = v.Estado,
                Creador = new UsuarioMinimoDto 
                { 
                    Id = v.Creador.Id, 
                    Nombre = v.Creador.Nombre, 
                    Avatar = v.Creador.AvatarIniciales 
                },
                Participantes = v.Participantes.Select(p => new UsuarioMinimoDto
                {
                    Id = p.Usuario.Id,
                    Nombre = p.Usuario.Nombre,
                    Avatar = p.Usuario.AvatarIniciales
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

   
    public async Task<List<ViajeResumenDto>> ObtenerViajesActivosAsync(int usuarioId)
    {
        return await _context.Viajes
            // Filtro: El viaje debe estar activo Y el usuario debe ser participante
            .Where(v => v.Estado == "Activo" && v.Participantes.Any(p => p.UsuarioId == usuarioId))
            .Select(v => new ViajeResumenDto
            {
                Id = v.Id,
                Nombre = v.Nombre,
                Ubicacion = v.Ubicacion,
                FechaInicio = v.FechaInicio,
                Estado = v.Estado,
                Creador = new UsuarioMinimoDto 
                { 
                    Id = v.Creador.Id, 
                    Nombre = v.Creador.Nombre, 
                    Avatar = v.Creador.AvatarIniciales 
                },
                Participantes = v.Participantes.Select(p => new UsuarioMinimoDto
                {
                    Id = p.Usuario.Id,
                    Nombre = p.Usuario.Nombre,
                    Avatar = p.Usuario.AvatarIniciales
                }).ToList()
            })
            .ToListAsync();
    }

   
    public async Task FinalizarViajeAsync(int viajeId, int usuarioId)
    {
        var viaje = await _context.Viajes
            .Include(v => v.Participantes).ThenInclude(p => p.Usuario)
            .Include(v => v.Gastos).ThenInclude(g => g.Detalles)
            .Include(v => v.Liquidaciones)
            .FirstOrDefaultAsync(v => v.Id == viajeId);

        if (viaje == null) throw new Exception("Viaje no encontrado");
        
        // B. Seguridad: Solo el creador cierra el viaje
        if (viaje.CreadorId != usuarioId) 
            throw new UnauthorizedAccessException("Solo el creador del viaje puede finalizarlo.");

        viaje.Estado = "Completado";
        viaje.FechaFin = DateTime.Now;
        _context.Viajes.Update(viaje);

        var liquidacionesPrevias = _context.Liquidaciones.Where(l => l.ViajeId == viajeId);
        _context.Liquidaciones.RemoveRange(liquidacionesPrevias);

        var liquidaciones = _liquidacionService.CalcularLiquidaciones(viaje);
        
        if (liquidaciones.Any())
        {
            _context.Liquidaciones.AddRange(liquidaciones);
        }

        await _context.SaveChangesAsync();
    }
}
