using Lecheria.Domain.Enums;

namespace Lecheria.Application.Dtos;

public class CrearOrdenoRequest
{
    public int ResId { get; set; }
    public DateOnly Fecha { get; set; }
    public Jornada Jornada { get; set; }
    public decimal CantidadLitros { get; set; }
}
