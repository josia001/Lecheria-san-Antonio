using System;

namespace Lecheria.Application.DTOs.Auth
{
    public class LoginRespuestaDto
    {
        // Token JWT para acceder a los endpoints protegidos
        public string Token { get; set; } = string.Empty;

        // Tipo de autenticación
        public string TipoToken { get; set; } = "Bearer";

        // Fecha y hora de vencimiento del token
        public DateTime FechaExpiracion { get; set; }

        // Identificador del usuario autenticado
        public int UsuarioId { get; set; }

        // Nombre completo del usuario
        public string NombreCompleto { get; set; } = string.Empty;

        // Correo electrónico
        public string Correo { get; set; } = string.Empty;

        // Identificador del rol
        public int RolId { get; set; }

        // Nombre del rol asignado
        public string RolNombre { get; set; } = string.Empty;
    }
}