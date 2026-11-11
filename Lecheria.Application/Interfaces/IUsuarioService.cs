using Lecheria.Application.Common;
using Lecheria.Application.DTOs.Usuarios;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lecheria.Application.Interfaces
{
    public interface IUsuarioService
    {
        // Registrar un nuevo usuario
        Task<ResultadoServicio<UsuarioRespuestaDto>> CrearAsync(
            CrearUsuarioDto dto);

        // Consultar todos los usuarios
        Task<ResultadoServicio<List<UsuarioRespuestaDto>>> ObtenerTodosAsync();

        // Consultar un usuario por su ID
        Task<ResultadoServicio<UsuarioRespuestaDto>> ObtenerPorIdAsync(
            int id);

        // Actualizar la información de un usuario
        Task<ResultadoServicio<UsuarioRespuestaDto>> ActualizarAsync(
            int id,
            ActualizarUsuarioDto dto);

        // Desactivar una cuenta sin eliminarla de la base de datos
        Task<ResultadoServicio<bool>> DesactivarAsync(
            int id);
    }
}