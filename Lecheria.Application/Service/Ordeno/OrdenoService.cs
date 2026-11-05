using Lecheria.Application.Dtos;
using Lecheria.Domain.Entities;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-14: registro de ordeno.
/// RN-09: la res debe existir y los litros deben ser mayores que cero.
/// </summary>
public class OrdenoService
{
    private readonly IOrdenoRepository _ordenos;
    private readonly IResRepository _reses;

    public OrdenoService(IOrdenoRepository ordenos, IResRepository reses)
    {
        _ordenos = ordenos;
        _reses = reses;
    }

    public async Task<IEnumerable<Ordeno>> ListarPorResAsync(int resId)
        => await _ordenos.FindAsync(o => o.ResId == resId);

    public async Task<Ordeno> RegistrarAsync(CrearOrdenoRequest solicitud)
    {
        _ = await _reses.GetByIdAsync(solicitud.ResId)
            ?? throw new NoEncontradoException("La res indicada no existe (RN-09).");

        if (solicitud.CantidadLitros <= 0)
            throw new AppException("La cantidad de litros debe ser mayor que cero (RN-09).");

        var ordeno = new Ordeno
        {
            ResId = solicitud.ResId,
            Fecha = solicitud.Fecha == default ? DateOnly.FromDateTime(DateTime.Today) : solicitud.Fecha,
            Jornada = solicitud.Jornada,
            CantidadLitros = solicitud.CantidadLitros
        };

        await _ordenos.AddAsync(ordeno);
        return ordeno;
    }
}
