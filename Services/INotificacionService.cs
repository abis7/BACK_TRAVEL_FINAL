namespace TravelFriend;

public interface INotificacionService
{
    Task<List<Notificacion>> ObtenerMisNotificacionesAsync(int usuarioId);
    Task<bool> MarcarComoLeidaAsync(int notificacionId, int usuarioId);
}
