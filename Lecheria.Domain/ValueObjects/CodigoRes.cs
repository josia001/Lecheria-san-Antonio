using System.Text.RegularExpressions;
using Lecheria.Domain.Exceptions;

namespace Lecheria.Domain.ValueObjects;

/// <summary>
/// Objeto de valor para el codigo de la res (RN-01).
/// Debe tener formato LESAN-#### y no puede estar vacio.
/// </summary>
public sealed class CodigoRes
{
    private static readonly Regex Patron = new(@"^LESAN-\d{4}$", RegexOptions.Compiled);

    public string Valor { get; }

    private CodigoRes(string valor) => Valor = valor;

    public static CodigoRes Crear(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ReglaDominioException("El codigo de la res es obligatorio (RN-02).");

        if (!Patron.IsMatch(codigo))
            throw new ReglaDominioException("El codigo debe tener el formato LESAN-0000 (RN-01).");

        return new CodigoRes(codigo);
    }

    public override string ToString() => Valor;
}
