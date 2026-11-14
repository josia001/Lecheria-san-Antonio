using Lecheria.Application.DTOs.Usuarios;
using Lecheria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lecheria.Web.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize(Policy = "SoloAdministrador")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // ==========================================
        // GET: api/usuarios
        // OBTENER TODOS LOS USUARIOS
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var resultado = await _usuarioService.ObtenerTodosAsync();

            if (!resultado.Exitoso)
            {
                return StatusCode(
                    resultado.CodigoEstado,
                    new { mensaje = resultado.Mensaje });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        // ==========================================
        // GET: api/usuarios/1
        // OBTENER USUARIO POR ID
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var resultado = await _usuarioService.ObtenerPorIdAsync(id);

            if (!resultado.Exitoso)
            {
                return StatusCode(
                    resultado.CodigoEstado,
                    new { mensaje = resultado.Mensaje });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        // ==========================================
        // POST: api/usuarios
        // CREAR USUARIO
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> Crear(
            [FromBody] CrearUsuarioDto dto)
        {
            var resultado = await _usuarioService.CrearAsync(dto);

            if (!resultado.Exitoso)
            {
                return StatusCode(
                    resultado.CodigoEstado,
                    new { mensaje = resultado.Mensaje });
            }

            return StatusCode(201, new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        // ==========================================
        // PUT: api/usuarios/1
        // ACTUALIZAR USUARIO
        // ==========================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(
            int id,
            [FromBody] ActualizarUsuarioDto dto)
        {
            var resultado = await _usuarioService.ActualizarAsync(id, dto);

            if (!resultado.Exitoso)
            {
                return StatusCode(
                    resultado.CodigoEstado,
                    new { mensaje = resultado.Mensaje });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }

        // ==========================================
        // PATCH: api/usuarios/1/desactivar
        // DESACTIVAR USUARIO
        // ==========================================

        [HttpPatch("{id:int}/desactivar")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var resultado = await _usuarioService.DesactivarAsync(id);

            if (!resultado.Exitoso)
            {
                return StatusCode(
                    resultado.CodigoEstado,
                    new { mensaje = resultado.Mensaje });
            }

            return NoContent();
        }
    }
}