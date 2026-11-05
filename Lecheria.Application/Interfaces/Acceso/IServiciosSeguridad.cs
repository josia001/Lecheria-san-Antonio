using Lecheria.Domain.Entities;

namespace Lecheria.Application.Interfaces;

/// <summary>Hashing de contrasenas (HU-01/HU-02). Nunca se guarda texto plano.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

/// <summary>Generacion del token de sesion JWT (HU-02).</summary>
public interface ITokenService
{
    string CrearToken(Usuario usuario);
}
