using Lecheria.Application.DTOs.Auth;
using Lecheria.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lecheria.Web.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ==========================================
        // POST: api/auth/login
        // ==========================================

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto)
        {
            var resultado = await _authService.LoginAsync(dto);

            if (!resultado.Exitoso)
            {
                return StatusCode(
                    resultado.CodigoEstado,
                    new
                    {
                        mensaje = resultado.Mensaje
                    });
            }

            return Ok(new
            {
                mensaje = resultado.Mensaje,
                datos = resultado.Datos
            });
        }
    }
}