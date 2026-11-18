using System.ComponentModel.DataAnnotations;

namespace Lecheria.Application.DTOs.Alimentacion;

public class AlimentacionDto
{
    [Range(1, int.MaxValue,
        ErrorMessage = "Debe seleccionar una res válida.")]
    public int ResId { get; set; }

    [Required(ErrorMessage = "El tipo de alimento es obligatorio.")]
    public string TipoAlimento { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.0001", "999999.9999",
        ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public decimal Cantidad { get; set; }

    [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
    public string UnidadMedida { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de suministro es obligatoria.")]
    public DateTime? FechaSuministro { get; set; }
}