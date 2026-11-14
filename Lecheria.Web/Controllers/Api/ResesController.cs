using Lecheria.Application.Dtos;
using Lecheria.Application.Exception;
using Lecheria.Application.Service;
using Lecheria.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Lecheria.Web.Controllers.Api;

[ApiController]
[Route("api/reses")]
public class ResesController(ResService reses) : ControllerBase
{
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ResResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResResponse>> Obtener(int id)
    {
        try
        {
            var res = await reses.ObtenerPorIdAsync(id);
            return Ok(CrearRespuesta(res));
        }
        catch (NoEncontradoException error)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound,
                title: "Res no encontrada.", detail: error.Message);
        }
    }

    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ResResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ResResponse>> Registrar([FromBody] CrearResRequest solicitud)
    {
        try
        {
            var res = await reses.RegistrarAsync(solicitud);
            return CreatedAtAction(nameof(Obtener), new { id = res.Id }, CrearRespuesta(res));
        }
        catch (AppException error)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest,
                title: "No se pudo registrar la res.", detail: error.Message);
        }
    }

    private static ResResponse CrearRespuesta(Res res) => new()
    {
        Id = res.Id,
        Codigo = res.Codigo,
        Raza = res.Raza,
        Sexo = res.Sexo,
        FechaNacimiento = res.FechaNacimiento,
        EdadAnios = res.EdadAnios,
        Peso = res.Peso,
        Estado = res.Estado
    };
}
