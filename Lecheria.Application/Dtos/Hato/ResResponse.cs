using Lecheria.Domain.Enums;

namespace Lecheria.Application.Dtos;

public sealed class ResResponse
{
    public int Id { get; init; }
    public string Codigo { get; init; } = string.Empty;
    public string Raza { get; init; } = string.Empty;
    public Sexo Sexo { get; init; }
    public DateOnly FechaNacimiento { get; init; }
    public int EdadAnios { get; init; }
    public decimal Peso { get; init; }
    public EstadoRes Estado { get; init; }
}
