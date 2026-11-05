namespace Lecheria.Domain.Entities;

/// <summary>
/// Cuenta de usuario del sistema (HU-01, HU-02).
/// La contrasena se almacena siempre como hash (nunca en texto plano).
/// </summary>
public class Usuario : EntityBase
{
    public string NombreUsuario { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;

    public int PerfilId { get; set; }
    public Perfil? Perfil { get; set; }
}
