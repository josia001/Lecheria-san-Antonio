using Lecheria.Application.Dtos;
using Lecheria.Domain.Entities;
using Lecheria.Domain.Enums;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;
using Lecheria.Application.Objetvalue;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-03 / HU-04: registro y consulta de la ficha de una res.
/// RN-01: el codigo de identificacion es unico en todo el hato.
/// </summary>
public class ResService
{
    private readonly IResRepository _reses;

    public ResService(IResRepository reses) => _reses = reses;

    public async Task<Res> ObtenerPorIdAsync(int id)
        => await _reses.GetByIdAsync(id)
           ?? throw new NoEncontradoException("Res no encontrada.");

    public async Task<Res> RegistrarAsync(CrearResRequest solicitud)
    {
        Validar(solicitud.Codigo, solicitud.Raza, solicitud.FechaNacimiento, solicitud.Peso, solicitud.Sexo, solicitud.Estado);

        var repetida = await _reses.GetByCodigoAsync(solicitud.Codigo.Trim());
        if (repetida is not null)
            throw new AppException($"Ya existe una res con el codigo '{solicitud.Codigo}' (RN-01).");

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

    private static void Validar(string codigo, string raza, DateOnly nacimiento, decimal peso, Sexo sexo, EstadoRes estado)
    {
        if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(raza))
            throw new AppException("Indique el código y la raza.");
        if (codigo.Trim().Length > Res.LongitudMaximaCodigo)
            throw new AppException($"El código de la res admite hasta {Res.LongitudMaximaCodigo} caracteres.");
        if (nacimiento == default || nacimiento > DateOnly.FromDateTime(DateTime.Today))
            throw new AppException("Indique una fecha de nacimiento válida.");
        if (!Enum.IsDefined(sexo) || !Enum.IsDefined(estado))
            throw new AppException("Indique un sexo y un estado válidos.");
        _ = Peso.Crear(peso);
    }
}
