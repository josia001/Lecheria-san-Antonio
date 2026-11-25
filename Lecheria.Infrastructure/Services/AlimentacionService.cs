using Lecheria.Application.DTOs.Alimentacion;
using Lecheria.Application.Interfaces.Alimentacion;
using Lecheria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure.Services;

public class AlimentacionService : IAlimentacionService
{
    private readonly LecheriaDb _context;

    public AlimentacionService(LecheriaDb context)
    {
        _context = context;
    }

    public async Task<Alimentacion> RegistrarAsync(
        AlimentacionDto dto)
    {
        // Verificar que la res exista.
        var resExiste = await _context.Set<Res>()
            .AnyAsync(r => r.Id == dto.ResId);

        if (!resExiste)
            throw new KeyNotFoundException(
                "La res seleccionada no existe.");

        // Validar el tipo de alimento.
        string[] alimentosPermitidos =
        {
            "Pasto",
            "Heno",
            "Concentrado",
            "Ensilaje",
            "Suplemento",
            "Melaza"
        };

        var tipoAlimento = alimentosPermitidos
            .FirstOrDefault(a => string.Equals(
                a, dto.TipoAlimento?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (tipoAlimento == null)
            throw new ArgumentException(
                "El tipo de alimento no es válido.");

        // Validar la cantidad.
        if (dto.Cantidad <= 0 ||
            dto.Cantidad > 999999.9999m ||
            decimal.Round(dto.Cantidad, 4) != dto.Cantidad)
            throw new ArgumentException(
                "La cantidad debe ser positiva y válida.");

        // Validar la unidad de medida.
        string[] unidadesPermitidas = { "kg", "lb", "g", "L" };

        var unidadMedida = unidadesPermitidas
            .FirstOrDefault(u => string.Equals(
                u, dto.UnidadMedida?.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (unidadMedida == null)
            throw new ArgumentException(
                "La unidad de medida no es válida.");

        // Validar la fecha.
        if (!dto.FechaSuministro.HasValue)
            throw new ArgumentException(
                "La fecha de suministro es obligatoria.");

        // Crear el registro de alimentación.
        var alimentacion = new Alimentacion
        {
            ResId = dto.ResId,
            TipoAlimento = tipoAlimento,
            Cantidad = dto.Cantidad,
            UnidadMedida = unidadMedida,
            FechaSuministro = dto.FechaSuministro.Value
        };

        // Guardar en la base de datos.
        _context.Alimentaciones.Add(alimentacion);

        await _context.SaveChangesAsync();

        return alimentacion;
    }
}