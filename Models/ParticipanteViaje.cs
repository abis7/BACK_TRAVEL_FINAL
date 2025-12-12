using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class ParticipanteViaje
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    [ForeignKey("UsuarioId")]
    public Usuario Usuario { get; set; }

    public int ViajeId { get; set; }
    [ForeignKey("ViajeId")]
    public Viaje Viaje { get; set; }

    public decimal Presupuesto { get; set; }
}
