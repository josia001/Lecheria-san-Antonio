using System.ComponentModel.DataAnnotations.Schema;
using Lecheria.Domain.Enums;

namespace Lecheria.Domain.Entities;


/// Ficha individual de la res (HU-03, HU-04, HU-05) y raiz del agregado Res.
/// el codigo de identificacion debe ser unico.

public class Res : EntityBase
{
    public string Codigo { get; set; } = string.Empty;
    public string Raza { get; set; } = string.Empty;
    public Sexo Sexo { get; set; }
    public DateOnly FechaNacimiento { get; set; }
    public decimal Peso { get; set; }
    public EstadoRes Estado { get; set; } = EstadoRes.Activa;

    public ICollection<Alimentacion> Alimentaciones { get; set; } = new List<Alimentacion>();
    public ICollection<EventoSanitario> EventosSanitarios { get; set; } = new List<EventoSanitario>();
    public ICollection<Ordeno> Ordenos { get; set; } = new List<Ordeno>();

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