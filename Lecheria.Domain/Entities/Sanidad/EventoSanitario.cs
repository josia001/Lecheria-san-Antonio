using Lecheria.Domain.Enums;

namespace Lecheria.Domain.Entities;

/// <summary>
/// Evento sanitario de una res: vacuna, tratamiento u observacion (HU-08).
/// RN-05: asociado a una res existente, con fecha, tipo y descripcion.
/// </summary>
public class EventoSanitario : EntityBase
{
    public int ResId { get; set; }
    public Res? Res { get; set; }

    public TipoEventoSanitario Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Producto { get; set; }
    public string? Dosis { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
