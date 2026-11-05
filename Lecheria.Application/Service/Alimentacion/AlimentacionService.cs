using Lecheria.Application.Dtos;
using Lecheria.Domain.Entities;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-06: registro de alimentacion por res.
/// RN-04: la res debe existir y la cantidad debe ser mayor que cero.
/// </summary>
public class AlimentacionService
{
    private readonly IAlimentacionRepository _alimentaciones;
    private readonly IResRepository _reses;

    public AlimentacionService(IAlimentacionRepository alimentaciones, IResRepository reses)
    {
        _alimentaciones = alimentaciones;
        _reses = reses;
    }

    public async Task<IEnumerable<Alimentacion>> ListarPorResAsync(int resId)
        => await _alimentaciones.FindAsync(a => a.ResId == resId);

    public async Task<Alimentacion> RegistrarAsync(CrearAlimentacionRequest solicitud)
    {
        _ = await _reses.GetByIdAsync(solicitud.ResId)
            ?? throw new NoEncontradoException("La res indicada no existe (RN-04).");

        if (string.IsNullOrWhiteSpace(solicitud.Alimento))
            throw new AppException("El alimento es obligatorio (RN-04).");

        if (solicitud.Cantidad <= 0)
            throw new AppException("La cantidad debe ser mayor que cero (RN-04).");

        var registro = new Alimentacion
        {
            ResId = solicitud.ResId,
            Alimento = solicitud.Alimento.Trim(),
            Cantidad = solicitud.Cantidad,
            Unidad = string.IsNullOrWhiteSpace(solicitud.Unidad) ? "kg" : solicitud.Unidad.Trim(),
            Fecha = solicitud.Fecha == default ? DateTime.UtcNow : solicitud.Fecha,
            Observacion = solicitud.Observacion
        };

        await _alimentaciones.AddAsync(registro);
        return registro;
    }
}
