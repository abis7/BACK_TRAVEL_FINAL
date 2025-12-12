namespace TravelFriend;

public interface IViajeService
{
    Task<Viaje> CrearViajeAsync(Viaje viaje, List<int> participantesIds);
    
    Task<ViajeResumenDto?> ObtenerViajeAsync(int viajeId);
    Task<List<ViajeResumenDto>> ObtenerViajesActivosAsync(int usuarioId);
    
    Task FinalizarViajeAsync(int viajeId, int usuarioId);
}

