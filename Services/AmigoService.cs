using Microsoft.EntityFrameworkCore;
namespace TravelFriend;

public class AmigoService : IAmigoService
{
    private readonly AppDbContext _context;

    public AmigoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> AgregarAmigoAsync(int miId, string emailAmigo)
    {

        var amigoUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == emailAmigo);

        if (amigoUser == null) return "No se encontró ningún usuario con ese email.";
        if (amigoUser.Id == miId) return "No puedes agregarte a ti mismo (amor propio, sí; amistad, no).";

        var yaSonAmigos = await _context.Amigos
            .AnyAsync(a => (a.UsuarioId == miId && a.AmigoId == amigoUser.Id) ||
                           (a.UsuarioId == amigoUser.Id && a.AmigoId == miId));

        if (yaSonAmigos) return "Ya son amigos.";

        var amistad = new Amigo
        {
            UsuarioId = miId,
            AmigoId = amigoUser.Id,
            FechaAmistad = DateTime.Now
        };

        _context.Amigos.Add(amistad);
        await _context.SaveChangesAsync();

        return "OK";
    }

    public async Task<List<AmigoDto>> ObtenerAmigosAsync(int idBusqueda)
    {
        var amistades = await _context.Amigos
            .Where(a => a.UsuarioId == idBusqueda || a.AmigoId == idBusqueda)
            .Include(a => a.Usuario)
            .Include(a => a.Amiguito)
            .ToListAsync();

        return amistades.Select(a =>
        {
            Usuario amigoUser = (a.UsuarioId == idBusqueda) ? a.Amiguito : a.Usuario;

            return new AmigoDto
            {
                Id = amigoUser.Id,
                Nombre = amigoUser.Nombre,
                UsuarioNombre = amigoUser.UsuarioNombre,
                Email = amigoUser.Email,
                Avatar = amigoUser.AvatarIniciales ?? "XX", // Fallback por si es nulo
                FechaAmistad = a.FechaAmistad
            };
        }).ToList();
    }

    public async Task<bool> EliminarAmistadAsync(int miId, int amigoId)
    {
        var amistad = await _context.Amigos
            .FirstOrDefaultAsync(a => (a.UsuarioId == miId && a.AmigoId == amigoId) ||
                                      (a.UsuarioId == amigoId && a.AmigoId == miId));

        if (amistad == null) return false;

        _context.Amigos.Remove(amistad);
        await _context.SaveChangesAsync();
        return true;
    }
}
