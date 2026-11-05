using Lecheria.Domain.Enums;

namespace Lecheria.Application.Dtos;

public class CrearEventoSanitarioRequest
{
    public int ResId { get; set; }
    public TipoEventoSanitario Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? Producto { get; set; }
    public string? Dosis { get; set; }
    public DateTime Fecha { get; set; }
}
