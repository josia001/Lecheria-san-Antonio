using Lecheria.Domain.Entities;

namespace Lecheria.Domain.Aggregates;

/// <summary>
/// Agregado Res: la res es la raiz y agrupa sus registros dependientes
/// (alimentacion, sanidad y ordeno). Mantiene la consistencia del conjunto
/// (referencia RN-01 a RN-09 del documento de requisitos).
/// </summary>
public class ResAggregate
{
    public Res Raiz { get; }

    public ResAggregate(Res raiz) => Raiz = raiz;

    public IEnumerable<Alimentacion> Alimentaciones => Raiz.Alimentaciones;
    public IEnumerable<EventoSanitario> EventosSanitarios => Raiz.EventosSanitarios;
    public IEnumerable<Ordeno> Ordenos => Raiz.Ordenos;

    /// <summary>Total de leche producida por la res (apoya HU-15).</summary>
    public decimal TotalLecheLitros => Raiz.Ordenos.Sum(o => o.CantidadLitros);
}
