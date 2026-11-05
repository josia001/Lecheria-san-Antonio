using Lecheria.Application.Dtos;
using Lecheria.Domain.Entities;
using Lecheria.Application.Exception;
using Lecheria.Application.Interfaces;

namespace Lecheria.Application.Service;

/// <summary>
/// HU-01: gestion de usuarios (crear, listar, activar/desactivar).
/// RN-12: el perfil define los permisos del usuario.
/// </summary>
public class UsuarioService
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IPerfilRepository _perfiles;
    private readonly IPasswordHasher _hasher;

    public UsuarioService(IUsuarioRepository usuarios, IPerfilRepository perfiles, IPasswordHasher hasher)
    {
        _usuarios = usuarios;
        _perfiles = perfiles;
        _hasher = hasher;
    }

    public Task<IEnumerable<Usuario>> ListarAsync() => _usuarios.GetAllAsync();

    public async Task<Usuario> CrearAsync(CrearUsuarioRequest solicitud)
    {
        if (string.IsNullOrWhiteSpace(solicitud.NombreUsuario))
            throw new AppException("El nombre de usuario es obligatorio.");

        if (solicitud.Password.Length < 6)
            throw new AppException("La contrasena debe tener al menos 6 caracteres.");

        var existente = await _usuarios.GetByNombreUsuarioAsync(solicitud.NombreUsuario);
        if (existente is not null)
            throw new AppException($"Ya existe un usuario con el nombre '{solicitud.NombreUsuario}'.");

        var perfil = await _perfiles.GetByIdAsync(solicitud.PerfilId)
            ?? throw new NoEncontradoException("El perfil indicado no existe.");

        var usuario = new Usuario
        {
            NombreUsuario = solicitud.NombreUsuario.Trim(),
            NombreCompleto = solicitud.NombreCompleto.Trim(),
            PasswordHash = _hasher.Hash(solicitud.Password),
            PerfilId = perfil.Id,
            Activo = true
        };

        await _usuarios.AddAsync(usuario);
        return usuario;
    }

    public async Task CambiarEstadoAsync(int id, bool activo)
    {
        var usuario = await _usuarios.GetByIdAsync(id)
            ?? throw new NoEncontradoException("Usuario no encontrado.");

        usuario.Activo = activo;
        await _usuarios.UpdateAsync(usuario);
    }
}
