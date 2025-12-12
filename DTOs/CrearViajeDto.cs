namespace TravelFriend;

public class CrearViajeDto
{
    public string Nombre { get; set; }
    public string Ubicacion { get; set; }
    public List<int> ParticipantesIds { get; set; } = new();
}
