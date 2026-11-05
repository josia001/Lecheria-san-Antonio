namespace Lecheria.Domain.Entities;


/// Suministro de alimentacion a una res.
/// destinatario, fecha, alimento y cantidad valida.

public class Alimentacion : EntityBase
{
    public int ResId { get; set; }
    public Res? Res { get; set; }

    public string Alimento { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Unidad { get; set; } = "kg";
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? Observacion { get; set; }
}
