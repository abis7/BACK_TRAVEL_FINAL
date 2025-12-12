using Microsoft.EntityFrameworkCore;
namespace TravelFriend;

public class NotificacionService: INotificacionService
{
private readonly AppDbContext _context;

    public NotificacionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notificacion>> ObtenerMisNotificacionesAsync(int usuarioId)
    {
        return await _context.Notificaciones
            .Where(n => n.UsuarioId == usuarioId)
            .OrderByDescending(n => n.Fecha)
            .Take(50) // Limitamos a las últimas 50 
            .ToListAsync();
    }

    public async Task<bool> MarcarComoLeidaAsync(int notificacionId, int usuarioId)
    {
        var notificacion = await _context.Notificaciones.FindAsync(notificacionId);
        
        // Seguridad: Verificar que la notificación pertenezca al usuario
        if (notificacion == null || notificacion.UsuarioId != usuarioId) 
            return false;

        notificacion.Leida = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
