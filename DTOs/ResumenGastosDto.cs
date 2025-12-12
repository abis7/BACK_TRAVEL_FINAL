namespace TravelFriend;

public class ResumenGastosDto
{
public int UsuarioId { get; set; }
    public string Nombre { get; set; }
    public decimal TotalPagado { get; set; }   
    public decimal TotalConsumido { get; set; }
    public decimal Diferencia => TotalPagado - TotalConsumido; // Positivo = Le deben, Negativo = Debe
}
