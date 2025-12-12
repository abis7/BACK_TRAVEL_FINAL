namespace TravelFriend;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();
    Task<UsuarioDto?> ObtenerPorIdAsync(int id);
    Task<Usuario> CrearUsuarioAsync(Usuario usuario);
    Task<Usuario?> ValidarLoginAsync(string usuario, string password);
    Task<string> EliminarUsuarioAsync(int id);
    Task<bool> ExisteUsuarioAsync(string nombre, string email);

}
