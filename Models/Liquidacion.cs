using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TravelFriend;

public class Liquidacion
{
[Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ViajeId { get; set; }
        [ForeignKey("ViajeId")]
        public Viaje Viaje { get; set; }

        public int DeudorId { get; set; }
        [ForeignKey("DeudorId")]
        public Usuario Deudor { get; set; }

        public int AcreedorId { get; set; }
        [ForeignKey("AcreedorId")]
        public Usuario Acreedor { get; set; }
        public decimal Monto { get; set; }
        public bool Pagado { get; set; } = false;

        public DateTime Fecha { get; set; } = DateTime.Now;
}
