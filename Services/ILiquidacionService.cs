namespace TravelFriend;

public interface ILiquidacionService
{
    List<Liquidacion> CalcularLiquidaciones(Viaje viaje);
    Task<List<LiquidacionResumenDto>> ObtenerLiquidacionesResumenAsync(int viajeId);
    Task<List<BalanceUsuarioDto>> ObtenerBalanceEnVivoAsync(int viajeId);
    Task<bool> MarcarDeudaPagadaAsync(int liquidacionId, int usuarioId);
    Task<List<ResumenGastosDto>> ObtenerResumenGastosAsync(int viajeId);
}
