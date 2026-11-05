using Lecheria.Application.Dtos;
using Lecheria.Domain.Entities;
using Lecheria.Domain.Enums;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-03 / HU-04 / HU-05: registro, consulta y actualizacion de reses.
/// RN-01: el codigo de identificacion es unico en todo el hato.
/// </summary>
public class ResService
{
    private readonly IResRepository _reses;

    public ResService(IResRepository reses) => _reses = reses;

    public Task<IEnumerable<Res>> ListarAsync() => _reses.GetAllAsync();

    public async Task<Res> ObtenerPorIdAsync(int id)
        => await _reses.GetFichaCompletaAsync(id)
           ?? throw new NoEncontradoException("Res no encontrada.");

    public async Task<Res> RegistrarAsync(CrearResRequest solicitud)
    {
        if (string.IsNullOrWhiteSpace(solicitud.Codigo))
            throw new AppException("El codigo es obligatorio (RN-01).");

        var repetida = await _reses.GetByCodigoAsync(solicitud.Codigo.Trim());
        if (repetida is not null)
            throw new AppException($"Ya existe una res con el codigo '{solicitud.Codigo}' (RN-01).");

        if (solicitud.FechaNacimiento > DateOnly.FromDateTime(DateTime.Today))
            throw new AppException("La fecha de nacimiento no puede ser futura.");

        var res = new Res
        {
            Codigo = solicitud.Codigo.Trim(),
            Raza = solicitud.Raza.Trim(),
            Sexo = solicitud.Sexo,
            FechaNacimiento = solicitud.FechaNacimiento,
            Peso = solicitud.Peso,
            Estado = solicitud.Estado
        };

        await _reses.AddAsync(res);
        return res;
    }

    public async Task<Res> ActualizarAsync(int id, ActualizarResRequest solicitud)
    {
        var res = await _reses.GetByIdAsync(id)
            ?? throw new NoEncontradoException("Res no encontrada.");

        if (!string.Equals(res.Codigo, solicitud.Codigo, StringComparison.OrdinalIgnoreCase))
        {
            var repetida = await _reses.GetByCodigoAsync(solicitud.Codigo.Trim());
            if (repetida is not null)
                throw new AppException($"Ya existe una res con el codigo '{solicitud.Codigo}' (RN-01).");
        }

        res.Codigo = solicitud.Codigo.Trim();
        res.Raza = solicitud.Raza.Trim();
        res.Sexo = solicitud.Sexo;
        res.FechaNacimiento = solicitud.FechaNacimiento;
        res.Peso = solicitud.Peso;
        res.Estado = solicitud.Estado;

        await _reses.UpdateAsync(res);
        return res;
    }

    public async Task EliminarAsync(int id)
    {
        var res = await _reses.GetByIdAsync(id)
            ?? throw new NoEncontradoException("Res no encontrada.");
        await _reses.DeleteAsync(res);
    }
}
