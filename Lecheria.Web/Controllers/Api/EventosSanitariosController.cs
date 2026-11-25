using Lecheria.Application.Dtos;
using Lecheria.Application.Exception;
using Lecheria.Application.Service;
using Lecheria.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lecheria.Web.Controllers.Api;

[ApiController]
[Authorize(Roles = "Veterinario,Administrador")]
[Route("api/reses/{resId:int}/eventos-sanitarios")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class EventosSanitariosController(EventoSanitarioService eventos) : ControllerBase
{
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(EventoSanitarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoSanitarioResponse>> Registrar(
        int resId, [FromBody] RegistrarEventoSanitarioRequest solicitud)
    {
        try
        {
            var evento = await eventos.RegistrarAsync(resId, solicitud);
            return CreatedAtAction(nameof(Obtener), new { resId, id = evento.Id }, CrearRespuesta(evento));
        }
        catch (NoEncontradoException error)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Res no encontrada.", detail: error.Message);
        }
        catch (AppException error)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                title: "No se pudo registrar el evento sanitario.", detail: error.Message);
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventoSanitarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoSanitarioResponse>> Obtener(int resId, int id)
    {
        try
        {
            return Ok(CrearRespuesta(await eventos.ObtenerAsync(resId, id)));
        }
        catch (NoEncontradoException error)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Evento sanitario no encontrado.", detail: error.Message);
        }
    }

    private static EventoSanitarioResponse CrearRespuesta(EventoSanitario evento) =>
        new(evento.Id, evento.ResId, evento.Tipo, DateOnly.FromDateTime(evento.Fecha), evento.Descripcion);
}
