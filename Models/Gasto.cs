using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class Gasto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int ViajeId { get; set; }
    [ForeignKey("ViajeId")]
    public Viaje Viaje { get; set; }

    public int UsuarioId { get; set; }
    [ForeignKey("UsuarioId")]
    public Usuario Usuario { get; set; }

    public string Categoria { get; set; }

    public string Descripcion { get; set; }

    public decimal Monto { get; set; }

    public string Modalidad { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;

    public ICollection<GastoDetalle> Detalles { get; set; }
}
