using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class Notificacion
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    [ForeignKey("UsuarioId")]
    public Usuario Usuario { get; set; }

    public string Mensaje { get; set; }

    public bool Leida { get; set; } = false;
    public DateTime Fecha { get; set; } = DateTime.Now;
}
