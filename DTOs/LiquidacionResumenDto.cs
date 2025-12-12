namespace TravelFriend;

public class LiquidacionResumenDto
{
    public int Id { get; set; }
    public decimal Monto { get; set; }
    public bool Pagado { get; set; }
    
    public UsuarioMinimoDto Deudor { get; set; }
    public UsuarioMinimoDto Acreedor { get; set; }

}
