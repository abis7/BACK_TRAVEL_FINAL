using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class Viaje
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Nombre { get; set; }

    public string Ubicacion { get; set; }

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public string Estado { get; set; } = "Activo";

    public int CreadorId { get; set; }
    [ForeignKey("CreadorId")]
    public Usuario Creador { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    public ICollection<ParticipanteViaje> Participantes { get; set; }
    public ICollection<Gasto> Gastos { get; set; }
    public ICollection<Liquidacion> Liquidaciones { get; set; }


}
