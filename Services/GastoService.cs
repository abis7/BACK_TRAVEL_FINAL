using Microsoft.EntityFrameworkCore;

namespace TravelFriend;

public class GastoService : IGastoService
{
    private readonly AppDbContext _context;

    public GastoService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> EsParticipanteAsync(int viajeId, int usuarioId)
    {
        return await _context.ParticipanteViajes
            .AnyAsync(p => p.ViajeId == viajeId && p.UsuarioId == usuarioId);
    }

public async Task<Gasto> RegistrarGastoAsync(int viajeId, int usuarioId, Gasto gasto, Dictionary<int, decimal>? distribucion)
        {
            gasto.ViajeId = viajeId;
            gasto.UsuarioId = usuarioId;
            gasto.Fecha = DateTime.Now;
            gasto.Detalles = new List<GastoDetalle>();

            if (gasto.Modalidad == "Igual")
            {
                var idsParticipantes = await _context.ParticipanteViajes
                    .Where(p => p.ViajeId == viajeId)
                    .Select(p => p.UsuarioId)
                    .ToListAsync();

                int cantidad = idsParticipantes.Count;
                if (cantidad > 0)
                {
                    decimal porcentajeBase = Math.Floor((100m / cantidad) * 100) / 100;
                    decimal sobrante = 100m - (porcentajeBase * cantidad);

                    for (int i = 0; i < cantidad; i++)
                    {
                        gasto.Detalles.Add(new GastoDetalle
                        {
                            UsuarioId = idsParticipantes[i],
                            Porcentaje = (i == 0) ? porcentajeBase + sobrante : porcentajeBase
                        });
                    }
                }
            }

            else if (gasto.Modalidad == "Porcentaje")
            {
                if (distribucion == null || !distribucion.Any())
                    throw new Exception("Para la modalidad 'Porcentaje', debes enviar la distribución de gastos.");

                decimal sumaTotal = distribucion.Values.Sum();
                if (sumaTotal < 99.9m || sumaTotal > 100.1m)
                    throw new Exception($"La distribución suma {sumaTotal}%. Debe sumar exactamente 100%.");

                foreach (var kvp in distribucion)
                {
                    gasto.Detalles.Add(new GastoDetalle
                    {
                        UsuarioId = kvp.Key,
                        Porcentaje = kvp.Value
                    });
                }
            }

            else if (gasto.Modalidad == "Ruleta")
            {
                if (distribucion != null)
                {
                    foreach (var kvp in distribucion)
                    {
                        gasto.Detalles.Add(new GastoDetalle
                        {
                            UsuarioId = kvp.Key,
                            Porcentaje = kvp.Value
                        });
                    }
                }
            }

            else if (gasto.Modalidad == "Yo invito" || gasto.Modalidad == "Juego")
            {
                gasto.Detalles.Add(new GastoDetalle
                {
                    UsuarioId = usuarioId,
                    Porcentaje = 100
                });
            }

            _context.Gastos.Add(gasto);
            await _context.SaveChangesAsync();

            var implicados = gasto.Detalles
                .Where(d => d.UsuarioId != usuarioId) // Notificar a todos menos al que pagó
                .Select(d => d.UsuarioId)
                .ToList();

            if (implicados.Any())
            {
                _context.Notificaciones.AddRange(implicados.Select(uid => new Notificacion
                {
                    UsuarioId = uid,
                    Fecha = DateTime.Now,
                    Leida = false,
                    Mensaje = $"Nuevo gasto ({gasto.Modalidad}): {gasto.Descripcion} ({gasto.Monto:C})."
                }));
                await _context.SaveChangesAsync();
            }

            return gasto;
        }


public async Task<Gasto> RegistrarGastoRuletaAsync(int viajeId, int usuarioPayerId, CrearGastoDto dto)
        {
            var participantes = await _context.ParticipanteViajes
                .Where(p => p.ViajeId == viajeId)
                .Select(p => p.UsuarioId)
                .ToListAsync();

            if (!participantes.Any()) throw new Exception("No hay participantes en este viaje para jugar.");

            var random = new Random();
            int idVictima = participantes[random.Next(participantes.Count)];

            var gasto = new Gasto
            {
                Categoria = dto.Categoria,
                Descripcion = dto.Descripcion, 
                Monto = dto.Monto,
                Modalidad = "Ruleta" 
            };

            var distribucionVictima = new Dictionary<int, decimal>
            {
                { idVictima, 100 }
            };

            return await RegistrarGastoAsync(viajeId, usuarioPayerId, gasto, distribucionVictima);
        }
    }

