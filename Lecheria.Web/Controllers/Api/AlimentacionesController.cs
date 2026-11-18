using Lecheria.Application.DTOs.Alimentacion;
using Lecheria.Application.Interfaces.Alimentacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lecheria.Web.Controllers.Api;

[ApiController]
[Route("api/alimentaciones")]
[Authorize]
public class AlimentacionesController : ControllerBase
{
    private readonly IAlimentacionService _alimentacionService;

    public AlimentacionesController(
        IAlimentacionService alimentacionService)
    {
        _alimentacionService = alimentacionService;
    }

    /// <summary>
    /// Registra la alimentación suministrada a una res (HU-06).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Registrar(
        [FromBody] AlimentacionDto dto)
    {
        try
        {
            var alimentacion =
                await _alimentacionService.RegistrarAsync(dto);

            return StatusCode(StatusCodes.Status201Created, new
            {
                mensaje = "Alimentación registrada correctamente.",
                id = alimentacion.Id,
                resId = alimentacion.ResId,
                tipoAlimento = alimentacion.TipoAlimento,
                cantidad = alimentacion.Cantidad,
                unidadMedida = alimentacion.UnidadMedida,
                fechaSuministro = alimentacion.FechaSuministro
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensaje = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }
}