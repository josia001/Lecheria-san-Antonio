namespace Lecheria.Domain.Entities;

/// <summary>Perfil o rol de acceso (HU-01). Define los permisos (RN-12).</summary>
public class Perfil : EntityBase
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
