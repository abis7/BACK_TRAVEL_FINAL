using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelFriend;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GastoController : ControllerBase
    {
        private readonly IGastoService _gastoService;
        private readonly AppDbContext _context;

        public GastoController(IGastoService gastoService, AppDbContext context)
        {
            _gastoService = gastoService;
            _context = context;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }

        // LISTAR GASTOS
        [HttpGet("viaje/{viajeId}")]
        public async Task<IActionResult> ObtenerGastosDeViaje(int viajeId)
        {
            int usuarioId = ObtenerUsuarioId();

            if (!await _gastoService.EsParticipanteAsync(viajeId, usuarioId))
                return Unauthorized(new { mensaje = "No eres participante de este viaje." });

            var gastos = await _context.Gastos
                .Where(g => g.ViajeId == viajeId)
                .OrderByDescending(g => g.Fecha)
                .Select(g => new
                {
                    g.Id,
                    g.ViajeId,
                    g.UsuarioId,
                    g.Categoria,
                    g.Descripcion,
                    g.Monto,
                    g.Modalidad,
                    g.Fecha
                })
                .ToListAsync();

            return Ok(gastos);
        }

        // RULETA PRIMERO
        [HttpPost("{viajeId}/ruleta")]
        public async Task<IActionResult> JugarRuletaYRegistrar(int viajeId, [FromBody] CrearGastoDto dto)
        {
            try
            {
                if (dto.Monto <= 0) return BadRequest(new { mensaje = "El monto debe ser mayor a 0." });
                if (string.IsNullOrWhiteSpace(dto.Descripcion)) return BadRequest(new { mensaje = "Falta descripcion." });

                int usuarioPayerId = ObtenerUsuarioId();

                var gastoCreado = await _gastoService.RegistrarGastoRuletaAsync(viajeId, usuarioPayerId, dto);

                var detalleVictima = gastoCreado.Detalles.FirstOrDefault(d => d.Porcentaje == 100);

                if (detalleVictima == null)
                    return Ok(new { mensaje = "Gasto registrado, pero no pude identificar a la victima en la respuesta." });

                var nombreVictima = await _context.Usuarios
                    .Where(u => u.Id == detalleVictima.UsuarioId)
                    .Select(u => u.Nombre)
                    .FirstOrDefaultAsync();

                return Ok(new
                {
                    mensaje = $"La ruleta ha girado! Tu pagaste, pero la deuda es de {nombreVictima}.",
                    victima = nombreVictima,
                    victimaId = detalleVictima.UsuarioId,
                    pagadorId = usuarioPayerId,
                    gasto = new
                    {
                        id = gastoCreado.Id,
                        monto = gastoCreado.Monto,
                        descripcion = gastoCreado.Descripcion
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // TOTAL GASTADO GLOBAL
        [HttpGet("total-gastado")]
        public async Task<IActionResult> ObtenerTotalGastado()
        {
            int usuarioId = ObtenerUsuarioId();

            var totalGastado = await _context.Gastos
                .Where(g => g.Viaje.Participantes.Any(p => p.UsuarioId == usuarioId))
                .SumAsync(g => (decimal?)g.Monto) ?? 0;

            return Ok(new { totalGastado });
        }

        // RESUMEN POR VIAJES - NUEVO ENDPOINT
        [HttpGet("resumen-por-viajes")]
        public async Task<IActionResult> ObtenerResumenPorViajes()
        {
            int usuarioId = ObtenerUsuarioId();

            var resumen = await _context.Viajes
                .Include(v => v.Gastos)
                .Include(v => v.Participantes)
                .Where(v => v.Participantes.Any(p => p.UsuarioId == usuarioId))
                .Select(v => new
                {
                    ViajeId = v.Id,
                    NombreViaje = v.Nombre,
                    Ubicacion = v.Ubicacion,
                    Estado = v.Estado,
                    TotalGastado = v.Gastos.Sum(g => (decimal?)g.Monto) ?? 0,
                    CantidadGastos = v.Gastos.Count(),
                    CantidadParticipantes = v.Participantes.Count(),
                    FechaCreacion = v.FechaCreacion
                })
                .OrderByDescending(v => v.TotalGastado)
                .ToListAsync();

            return Ok(resumen);
        }

        // REGISTRAR DESPUES
        [HttpPost("{viajeId}/registrar")]
        public async Task<IActionResult> RegistrarGasto(int viajeId, [FromBody] CrearGastoDto dto)
        {
            int usuarioId = ObtenerUsuarioId();

            if (!await _gastoService.EsParticipanteAsync(viajeId, usuarioId))
                return Unauthorized(new { mensaje = "No puedes registrar gastos en un viaje del que no eres parte." });

            var gasto = new Gasto
            {
                Categoria = dto.Categoria,
                Descripcion = dto.Descripcion,
                Monto = dto.Monto,
                Modalidad = dto.Modalidad
            };

            try
            {
                var creado = await _gastoService.RegistrarGastoAsync(viajeId, usuarioId, gasto, dto.Distribucion);
                return Ok(new { mensaje = "Gasto registrado exitosamente", gastoId = creado.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = "Error al registrar gasto", error = ex.Message });
            }
        }
    }
}