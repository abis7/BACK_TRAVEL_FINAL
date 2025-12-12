using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using TravelFriend;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LiquidacionController : ControllerBase
    {
        private readonly ILiquidacionService _liquidacionService;

        public LiquidacionController(ILiquidacionService liquidacionService) => _liquidacionService = liquidacionService;

   private int ObtenerUsuarioId()
        {
            var claim = User.Claims.FirstOrDefault(c => 
                c.Type == "sub" || 
                c.Type == ClaimTypes.NameIdentifier);

            if (claim == null) 
            {
                throw new UnauthorizedAccessException("No se encontró el ID del usuario en el Token.");
            }

            if (!int.TryParse(claim.Value, out int id))
            {
                throw new UnauthorizedAccessException("El ID del token no es un número válido.");
            }

            return id;
        }
        [HttpGet("{viajeId}")]
        public async Task<IActionResult> ObtenerLiquidacionesFinales(int viajeId)
        {
            return Ok(await _liquidacionService.ObtenerLiquidacionesResumenAsync(viajeId));
        }

        [HttpGet("{viajeId}/balance")]
        public async Task<IActionResult> ObtenerBalanceEnVivo(int viajeId)
        {
            return Ok(await _liquidacionService.ObtenerBalanceEnVivoAsync(viajeId));
        }

        [HttpPost("{id}/pagar")]
        public async Task<IActionResult> MarcarPagado(int id)
        {
            try
            {
                int usuarioId = ObtenerUsuarioId();

                var exito = await _liquidacionService.MarcarDeudaPagadaAsync(id, usuarioId);
                
                if (exito) 
                    return Ok(new { mensaje = "Deuda marcada como PAGADA." });
                
                return NotFound(new { mensaje = "La deuda no existe o no tienes permiso para gestionarla." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{viajeId}/estadisticas")]
        public async Task<IActionResult> ObtenerEstadisticas(int viajeId)
        {
            var resumen = await _liquidacionService.ObtenerResumenGastosAsync(viajeId);
            return Ok(resumen);
        }
    }
}

