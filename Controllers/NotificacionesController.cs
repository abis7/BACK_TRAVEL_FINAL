using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TravelFriend;

namespace MyApp.Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacionesController : ControllerBase
    {
        private readonly INotificacionService _notifService;

        public NotificacionesController(INotificacionService notifService)
        {
            _notifService = notifService;
        }

        private int ObtenerUsuarioId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(claim!.Value);
        }

        [HttpGet]
        public async Task<IActionResult> MisNotificaciones()
        {
            var notificaciones = await _notifService.ObtenerMisNotificacionesAsync(ObtenerUsuarioId());
            return Ok(notificaciones);
        }

        [HttpPost("{id}/leer")]
        public async Task<IActionResult> MarcarLeida(int id)
        {
            var exito = await _notifService.MarcarComoLeidaAsync(id, ObtenerUsuarioId());

            if (exito) return Ok(); 
            return NotFound();      
        }
    }
}
