using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TravelFriend;

namespace MyApp.Namespace
{
    // DTO para recibir la lista de pagos desde el modal del frontend
    public class FinalizarViajeDto
    {
        public List<int> LiquidacionesPagadasIds { get; set; } = new();
    }

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ViajeController : ControllerBase
    {
        private readonly IViajeService _viajeService;
        private readonly ILiquidacionService _liquidacionService; // Agregado para procesar pagos

        // Inyectamos ambos servicios
        public ViajeController(IViajeService viajeService, ILiquidacionService liquidacionService)
        {
            _viajeService = viajeService;
            _liquidacionService = liquidacionService;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.Claims.FirstOrDefault(c =>
                c.Type == "sub" ||
                c.Type == ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                throw new UnauthorizedAccessException("No se encontró el ID de usuario en el token.");
            }

            if (!int.TryParse(claim.Value, out int id))
            {
                throw new UnauthorizedAccessException("El ID del token no es un número válido.");
            }

            return id;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearViaje([FromBody] CrearViajeDto dto)
        {
            try
            {
                int usuarioId = ObtenerUsuarioId();

                var viaje = new Viaje
                {
                    Nombre = dto.Nombre,
                    Ubicacion = dto.Ubicacion,
                    CreadorId = usuarioId
                };

                var creado = await _viajeService.CrearViajeAsync(viaje, dto.ParticipantesIds);

                return Ok(new
                {
                    mensaje = "Viaje creado exitosamente.",
                    viajeId = creado.Id,
                    nombre = creado.Nombre
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Ocurrió un error interno.", error = ex.Message });
            }
        }

        [HttpGet("{viajeId}")]
        public async Task<IActionResult> ObtenerViaje(int viajeId)
        {
            try
            {
                var viajeDto = await _viajeService.ObtenerViajeAsync(viajeId);

                if (viajeDto == null) return NotFound(new { mensaje = "Viaje no encontrado" });

                int usuarioId = ObtenerUsuarioId();
                bool soyParticipante = viajeDto.Participantes.Any(p => p.Id == usuarioId);

                if (!soyParticipante)
                {
                    return Unauthorized(new { mensaje = "No tienes permiso para ver este viaje." });
                }

                return Ok(viajeDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("activos")]
        public async Task<IActionResult> ObtenerMisViajesActivos()
        {
            int usuarioId = ObtenerUsuarioId();
            var viajes = await _viajeService.ObtenerViajesActivosAsync(usuarioId);
            return Ok(viajes);
        }


        [HttpPost("{viajeId}/finalizar")]
        public async Task<IActionResult> FinalizarViaje(int viajeId, [FromBody] FinalizarViajeDto dto)
        {
            try
            {
                int usuarioId = ObtenerUsuarioId();

                // 1. Finalizar el viaje (Esto genera las liquidaciones en la base de datos)
                await _viajeService.FinalizarViajeAsync(viajeId, usuarioId);

                // 2. Procesar los pagos marcados en el Frontend
                // Solo procesamos si hay IDs en la lista
                if (dto.LiquidacionesPagadasIds != null && dto.LiquidacionesPagadasIds.Any())
                {
                    foreach (var idLiq in dto.LiquidacionesPagadasIds)
                    {
                        // IMPORTANTE: Aquí usamos el servicio de liquidación para marcar 
                        // cada deuda como pagada. El try-catch interno evita que un error
                        try
                        {
                            await _liquidacionService.MarcarDeudaPagadaAsync(idLiq, usuarioId);
                        }
                        catch
                        {

                        }
                    }
                }

                return Ok(new { mensaje = "Viaje finalizado y liquidaciones actualizadas correctamente." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}