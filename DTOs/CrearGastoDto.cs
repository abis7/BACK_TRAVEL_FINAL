namespace TravelFriend;

public class CrearGastoDto
{
public string Descripcion { get; set; }
    public decimal Monto { get; set; }
    public string Categoria { get; set; }
    public string Modalidad { get; set; }
    
    // Asegúrate de que esto sea público
    public Dictionary<int, decimal>? Distribucion { get; set; }
}
