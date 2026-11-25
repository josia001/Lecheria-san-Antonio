using Lecheria.Application.Dtos;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;
using Lecheria.Domain.Entities;
using Lecheria.Domain.Interfaces;

namespace Lecheria.Application.Service;

public class EventoSanitarioService(
    IResRepository reses, IRepository<EventoSanitario> eventos)
{
    public async Task<EventoSanitario> RegistrarAsync(
        int resId, RegistrarEventoSanitarioRequest solicitud)
    {
        if (solicitud.Fecha == default)
            throw new AppException("Indique la fecha del evento sanitario.");
        if (!Enum.IsDefined(solicitud.Tipo))
            throw new AppException("Indique un tipo de evento sanitario válido.");
        if (string.IsNullOrWhiteSpace(solicitud.Descripcion))
            throw new AppException("Indique la descripción del evento sanitario.");
        if (await reses.GetByIdAsync(resId) is null)
            throw new NoEncontradoException("Res no encontrada.");

        var evento = new EventoSanitario
        {
            ResId = resId,
            Tipo = solicitud.Tipo,
            Fecha = solicitud.Fecha.ToDateTime(TimeOnly.MinValue),
            Descripcion = solicitud.Descripcion.Trim()
        };

        await eventos.AddAsync(evento);
        return evento;
    }

    public async Task<EventoSanitario> ObtenerAsync(int resId, int id)
    {
        var evento = await eventos.GetByIdAsync(id);
        if (evento is null || evento.ResId != resId)
            throw new NoEncontradoException("Evento sanitario no encontrado para esta res.");

        return evento;
    }
}
