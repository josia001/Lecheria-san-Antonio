using System.ComponentModel.DataAnnotations.Schema;
using Lecheria.Domain.Enums;

namespace Lecheria.Domain.Entities;

/// <summary>
/// Ficha individual de la res para su registro y consulta (HU-03, HU-04).
/// RN-01: el codigo de identificacion debe ser unico.
/// </summary>
public class Res : EntityBase
{
    public const int LongitudMaximaCodigo = 120;

    public string Codigo { get; set; } = string.Empty;
    public string Raza { get; set; } = string.Empty;
    public Sexo Sexo { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public EstadoRes Estado { get; set; } = EstadoRes.Activa;

    /// <summary>Edad en anos calculada desde la fecha de nacimiento.</summary>
    [NotMapped]
    public int EdadAnios
    {
        get
        {
            var hoy = DateOnly.FromDateTime(DateTime.Today);
            var edad = hoy.Year - FechaNacimiento.Year;
            if (FechaNacimiento > hoy.AddYears(-edad)) edad--;
            return edad < 0 ? 0 : edad;
        }
    }
}
