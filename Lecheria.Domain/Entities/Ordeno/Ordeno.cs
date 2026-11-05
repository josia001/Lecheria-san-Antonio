using Lecheria.Domain.Enums;

namespace Lecheria.Domain.Entities;


/// Jornada de ordeno de una res (HU-14).
/// fecha, jornada y res existente; volumen mayor que cero.

public class Ordeno : EntityBase
{
    public int ResId { get; set; }
    public Res? Res { get; set; }

    public DateOnly Fecha { get; set; }
    public Jornada Jornada { get; set; }
    public decimal CantidadLitros { get; set; }
}
