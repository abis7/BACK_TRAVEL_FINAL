using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TravelFriend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AmigoController : ControllerBase
    {
        private readonly IAmigoService _amigoService;

        public AmigoController(IAmigoService amigoService)
        {
            _amigoService = amigoService;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }

        [HttpPost("agregar")]
        public async Task<IActionResult> AgregarAmigo([FromBody] AgregarAmigoDto dto)
        {
            var resultado = await _amigoService.AgregarAmigoAsync(ObtenerUsuarioId(), dto.EmailAmigo);

            if (resultado == "OK")
                return Ok(new { mensaje = "¡Amistad creada exitosamente!" });

            if (resultado.Contains("No se encontró"))
                return NotFound(new { mensaje = resultado });

            return BadRequest(new { mensaje = resultado });
        }

        [HttpGet("amigos")]
        public async Task<IActionResult> ObtenerMisAmigos()
        {
            var amigos = await _amigoService.ObtenerAmigosAsync(ObtenerUsuarioId());
            return Ok(amigos);
        }

        [HttpGet("AmigoDeUsuario/{usuarioId}")]
        public async Task<IActionResult> ObtenerAmigosDeUsuario(int usuarioId)
        {
            var amigos = await _amigoService.ObtenerAmigosAsync(usuarioId);
            return Ok(amigos);
        }

        [HttpDelete("eliminar/{amigoId}")]
        public async Task<IActionResult> EliminarAmigo(int amigoId)
        {
            var exito = await _amigoService.EliminarAmistadAsync(ObtenerUsuarioId(), amigoId);

            if (exito)
                return Ok(new { mensaje = "Amistad eliminada." });

            return NotFound(new { mensaje = "No eran amigos o el usuario no existe." });
        }
    }
}