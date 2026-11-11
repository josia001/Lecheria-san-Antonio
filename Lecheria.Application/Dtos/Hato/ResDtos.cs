using Lecheria.Domain.Enums;

namespace Lecheria.Application.Dtos;

public class CrearResRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Raza { get; set; } = string.Empty;
    public Sexo Sexo { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public EstadoRes Estado { get; set; } = EstadoRes.Activa;
}
