using Lecheria.Domain.Enums;

namespace Lecheria.Domain.Entities;

public class EventoSanitario : EntityBase
{
    public int ResId { get; set; }
    public TipoEventoSanitario Tipo { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}
