using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class GastoDetalle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int GastoId { get; set; }
    [ForeignKey("GastoId")]
    public Gasto Gasto { get; set; }

    public int UsuarioId { get; set; }
    [ForeignKey("UsuarioId")]
    public Usuario Usuario { get; set; }

    public decimal Porcentaje { get; set; }
}
