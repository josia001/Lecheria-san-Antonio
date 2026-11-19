using Lecheria.Domain.Enums;

namespace Lecheria.Application.Dtos;

public sealed class RegistrarEventoSanitarioRequest
{
    public TipoEventoSanitario Tipo { get; set; }
    public DateOnly Fecha { get; set; }
    public string Descripcion { get; set; } = string.Empty;
}

public sealed record EventoSanitarioResponse(
    int Id, int ResId, TipoEventoSanitario Tipo, DateOnly Fecha, string Descripcion);
