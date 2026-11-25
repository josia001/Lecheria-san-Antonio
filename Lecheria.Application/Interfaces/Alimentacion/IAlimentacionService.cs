using Lecheria.Application.DTOs.Alimentacion;
using Lecheria.Domain.Entities;

namespace Lecheria.Application.Interfaces.Alimentacion;

public interface IAlimentacionService
{
    Task<Lecheria.Domain.Entities.Alimentacion> RegistrarAsync(
        AlimentacionDto dto);
}