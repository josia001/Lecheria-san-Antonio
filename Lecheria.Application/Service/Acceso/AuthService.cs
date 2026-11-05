using Lecheria.Application.Dtos;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-02: autenticacion de usuarios.
/// Valida credenciales contra el hash almacenado y devuelve un token JWT.
/// </summary>
public class AuthService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public AuthService(IUsuarioRepository usuarios, IPasswordHasher hasher, ITokenService tokens)
    {
        _usuarios = usuarios;
        _hasher = hasher;
        _tokens = tokens;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest solicitud)
    {
        var usuario = await _usuarios.GetByNombreUsuarioAsync(solicitud.NombreUsuario)
            ?? throw new NoEncontradoException("Usuario o contrasena incorrectos.");

        if (!usuario.Activo)
            throw new AppException("La cuenta se encuentra inactiva.");

        if (!_hasher.Verify(solicitud.Password, usuario.PasswordHash))
            throw new NoEncontradoException("Usuario o contrasena incorrectos.");

        var token = _tokens.CrearToken(usuario);
        return new LoginResponse
        {
            Token = token,
            NombreUsuario = usuario.NombreUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Perfil = usuario.Perfil?.Nombre ?? string.Empty
        };
    }
}
