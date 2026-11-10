using Lecheria.Application.Exception;

namespace Lecheria.Application.Objetvalue;

/// <summary>Objeto de valor para el peso de una res (RN-03). Debe ser positivo.</summary>
public sealed class Peso
{
    public decimal Valor { get; }

    private Peso(decimal valor) => Valor = valor;

    public static Peso Crear(decimal valor)
    {
        if (valor <= 0)
            throw new AppException("El peso debe ser mayor que cero (RN-03).");
        if (valor > 2000)
            throw new AppException("El peso excede el maximo permitido para el ganado.");
        return new Peso(valor);
    }

    public override string ToString() => $"{Valor:0.##}";
}
