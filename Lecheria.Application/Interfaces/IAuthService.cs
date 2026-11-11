using Lecheria.Application.Common;
using Lecheria.Application.DTOs.Auth;
using System.Threading.Tasks;

namespace Lecheria.Application.Interfaces
{
    public interface IAuthService
    {
        // Validar credenciales y generar un token JWT
        Task<ResultadoServicio<LoginRespuestaDto>> LoginAsync(
            LoginDto dto);
    }
}