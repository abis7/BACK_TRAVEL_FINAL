using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class Usuario
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Nombre { get; set; }

    public string UsuarioNombre { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string AvatarIniciales { get; set; }

    public string AvatarColor{ get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public List<Amigo> Amigos { get; set; } = new();
    public List<Gasto> Gastos { get; set; } = new();
    public List<Viaje> ViajesCreados { get; set; } = new();
    public List<Notificacion> Notificaciones { get; set; } = new();
    public List<ParticipanteViaje> Participaciones { get; set; } = new();
    public List<Liquidacion> LiquidacionesDeudor { get; set; } = new();
    public List<Liquidacion> LiquidacionesAcreedor { get; set; } = new();
}
