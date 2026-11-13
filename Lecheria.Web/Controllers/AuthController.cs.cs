using System;
using System.Security.Claims;
using System.Threading.Tasks;
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

        // ==========================================
        // POST: api/auth/logout
        // ==========================================

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Obtener identificador único del JWT
            string? jti = User.FindFirstValue("jti");

            if (string.IsNullOrWhiteSpace(jti))
            {
                return Unauthorized(new
                {
                    mensaje = "El token no contiene un identificador válido."
                });
            }

            // Obtener fecha de expiración del JWT
            string? expTexto = User.FindFirstValue("exp");

            if (!long.TryParse(expTexto, out long expUnix))
            {
                return Unauthorized(new
                {
                    mensaje = "El token no contiene una fecha de expiración válida."
                });
            }

            DateTime fechaExpiracion;

            try
            {
                fechaExpiracion = DateTimeOffset
                    .FromUnixTimeSeconds(expUnix)
                    .UtcDateTime;
            }
            catch (ArgumentOutOfRangeException)
            {
                return Unauthorized(new
                {
                    mensaje = "La fecha de expiración del token no es válida."
                });
            }

            // Revocar token en base de datos
            var resultado = await _authService.LogoutAsync(
                jti,
                fechaExpiracion);

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
                mensaje = resultado.Mensaje
            });
        }
    }
}