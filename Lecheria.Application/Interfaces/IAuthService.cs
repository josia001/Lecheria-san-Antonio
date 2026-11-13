using Lecheria.Application.Common;
using Lecheria.Application.DTOs.Auth;
using System;
using System.Threading.Tasks;

namespace Lecheria.Application.Interfaces
{
    public interface IAuthService
    {
        // Iniciar sesión y generar JWT
        Task<ResultadoServicio<LoginRespuestaDto>> LoginAsync(
            LoginDto dto);

        // Cerrar sesión y revocar JWT
        Task<ResultadoServicio<bool>> LogoutAsync(
            string jti,
            DateTime fechaExpiracion);
    }
}