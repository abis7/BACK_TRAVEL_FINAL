using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace TravelFriend;

public class UsuarioService : IUsuarioService
{
    private readonly AppDbContext _context;

    public UsuarioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioDto>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.Select(u => new UsuarioDto(u)).ToListAsync();
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);
        return user == null ? null : new UsuarioDto(user);
    }

    public async Task<Usuario> CrearUsuarioAsync(Usuario usuario)
    {
        // Esto genera el hash y le agrega seguridad "Salt" automáticamente
        usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task<bool> ExisteUsuarioAsync(string nombre, string email)
    {
        return await _context.Usuarios.AnyAsync(u => u.UsuarioNombre == nombre || u.Email == email);
    }

    public async Task<Usuario?> ValidarLoginAsync(string usuario, string password)
    {
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioNombre == usuario);

        if (user == null) return null;

        bool esCorrecta = BCrypt.Net.BCrypt.Verify(password, user.Password);

        if (!esCorrecta) return null;

        return user;
    }

    public async Task<string> EliminarUsuarioAsync(int id)
    {
        var user = await _context.Usuarios.FindAsync(id);
        if (user == null) return "Usuario no encontrado.";

        var viajeCreado = await _context.Viajes
            .FirstOrDefaultAsync(v => v.CreadorId == id && v.Estado == "Activo");

        if (viajeCreado != null)
        {
            return $"No puedes eliminar tu cuenta porque eres el creador del viaje activo '{viajeCreado.Nombre}'. Debes finalizarlo o salirte primero.";
        }

   
        var participacionActiva = await _context.ParticipanteViajes
            .Include(p => p.Viaje)
            .FirstOrDefaultAsync(p => p.UsuarioId == id && p.Viaje.Estado == "Activo");

        if (participacionActiva != null)
        {
            return $"No puedes eliminar tu cuenta porque perteneces al viaje '{participacionActiva.Viaje.Nombre}'.";
        }

        var tieneDeudas = await _context.Liquidaciones
            .AnyAsync(l => (l.DeudorId == id || l.AcreedorId == id) && !l.Pagado);

        if (tieneDeudas)
        {
            return "No puedes eliminar tu cuenta porque tienes deudas o cobros pendientes en viajes finalizados.";
        }

        var amigos = _context.Amigos.Where(a => a.UsuarioId == id || a.AmigoId == id);
        _context.Amigos.RemoveRange(amigos);

        var notifs = _context.Notificaciones.Where(n => n.UsuarioId == id);
        _context.Notificaciones.RemoveRange(notifs);

        _context.Usuarios.Remove(user);
        await _context.SaveChangesAsync();

        return "OK";
    }

}
