namespace TravelFriend;

public interface IAmigoService
{
    Task<string> AgregarAmigoAsync(int miId, string emailAmigo);
    Task<List<AmigoDto>> ObtenerAmigosAsync(int usuarioId);
    Task<bool> EliminarAmistadAsync(int miId, int amigoId);

}
