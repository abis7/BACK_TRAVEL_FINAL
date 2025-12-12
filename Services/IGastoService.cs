namespace TravelFriend;

public interface IGastoService
{
Task<bool> EsParticipanteAsync(int viajeId, int usuarioId);

Task<Gasto> RegistrarGastoAsync(int viajeId, int usuarioId, Gasto gasto, Dictionary<int, decimal>? distribucion);

Task<Gasto> RegistrarGastoRuletaAsync(int viajeId, int usuarioPayerId, CrearGastoDto dto);
}
