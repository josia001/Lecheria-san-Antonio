using Lecheria.Application.Dtos;
using Lecheria.Domain.Entities;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-08: registro de eventos sanitarios (vacunas, tratamientos).
/// RN-05: la res debe existir y la descripcion es obligatoria.
/// </summary>
public class SanidadService
{
    private readonly IEventoSanitarioRepository _eventos;
    private readonly IResRepository _reses;

    public SanidadService(IEventoSanitarioRepository eventos, IResRepository reses)
    {
        _eventos = eventos;
        _reses = reses;
    }

    public async Task<IEnumerable<EventoSanitario>> ListarPorResAsync(int resId)
        => await _eventos.FindAsync(e => e.ResId == resId);

    public async Task<EventoSanitario> RegistrarAsync(CrearEventoSanitarioRequest solicitud)
    {
        _ = await _reses.GetByIdAsync(solicitud.ResId)
            ?? throw new NoEncontradoException("La res indicada no existe (RN-05).");

        if (string.IsNullOrWhiteSpace(solicitud.Descripcion))
            throw new AppException("La descripcion del evento es obligatoria (RN-05).");

        var evento = new EventoSanitario
        {
            ResId = solicitud.ResId,
            Tipo = solicitud.Tipo,
            Descripcion = solicitud.Descripcion.Trim(),
            Producto = solicitud.Producto,
            Dosis = solicitud.Dosis,
            Fecha = solicitud.Fecha == default ? DateTime.UtcNow : solicitud.Fecha
        };

        await _eventos.AddAsync(evento);
        return evento;
    }
}
