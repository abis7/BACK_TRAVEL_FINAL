using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class Amigo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    // Usuario que envía la amistad
    public int UsuarioId { get; set; }
    [ForeignKey("UsuarioId")]
    public Usuario Usuario { get; set; }

    // Usuario que recibe la amistad
    public int AmigoId { get; set; }
    [ForeignKey("AmigoId")]
    public Usuario Amiguito { get; set; }

    public DateTime FechaAmistad { get; set; } = DateTime.Now;
}
