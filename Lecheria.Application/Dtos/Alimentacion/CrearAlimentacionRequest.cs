namespace Lecheria.Application.Dtos;

public class CrearAlimentacionRequest
{
    public int ResId { get; set; }
    public string Alimento { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Unidad { get; set; } = "kg";
    public DateTime Fecha { get; set; }
    public string? Observacion { get; set; }
}
