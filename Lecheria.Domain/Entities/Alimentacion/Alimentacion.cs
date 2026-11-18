namespace Lecheria.Domain.Entities;

/// <summary>
/// Registro de la alimentación suministrada
/// individualmente a una res (HU-06).
/// </summary>
public class Alimentacion : EntityBase
{
    public int ResId { get; set; }

    public Res Res { get; set; } = null!;

    public string TipoAlimento { get; set; } = string.Empty;

    public decimal Cantidad { get; set; }

    public string UnidadMedida { get; set; } = string.Empty;

    public DateTime FechaSuministro { get; set; }
}