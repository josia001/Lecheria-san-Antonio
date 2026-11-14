using System.ComponentModel.DataAnnotations;

namespace Lecheria.Application.DTOs.Usuarios
{
    public class ActualizarUsuarioDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        [MaxLength(150)]
        public string Correo { get; set; } = string.Empty;

        [Range(1, int.MaxValue,
            ErrorMessage = "Debe seleccionar un rol válido")]
        public int RolId { get; set; }
    }
}