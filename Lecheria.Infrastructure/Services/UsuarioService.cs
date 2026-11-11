using Lecheria.Application.Common;
using Lecheria.Application.DTOs.Usuarios;
using Lecheria.Application.Interfaces;
using Lecheria.Domain.Entities;
using Lecheria.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly LecheriaDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UsuarioService(
            LecheriaDbContext context,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ==========================================
        // REGISTRAR USUARIO
        // ==========================================

        public async Task<ResultadoServicio<UsuarioRespuestaDto>> CrearAsync(
            CrearUsuarioDto dto)
        {
            string correo = dto.Correo.Trim().ToLowerInvariant();

            bool correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo == correo);

            if (correoExiste)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "Ya existe un usuario registrado con este correo.",
                    409);
            }

            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.RolId && r.Activo);

            if (rol == null)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "El rol seleccionado no existe o está inactivo.",
                    400);
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Correo = correo,
                RolId = dto.RolId,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            usuario.PasswordHash = _passwordHasher.HashPassword(
                usuario,
                dto.Password);

            _context.Usuarios.Add(usuario);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "No se pudo registrar el usuario. Verifique que el correo no esté duplicado.",
                    409);
            }

            return ResultadoServicio<UsuarioRespuestaDto>.Creado(
                MapearUsuario(usuario, rol.Nombre),
                "Usuario registrado correctamente.");
        }

        // ==========================================
        // CONSULTAR TODOS LOS USUARIOS
        // ==========================================

        public async Task<ResultadoServicio<List<UsuarioRespuestaDto>>>
            ObtenerTodosAsync()
        {
            var usuarios = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .OrderBy(u => u.Id)
                .ToListAsync();

            var resultado = usuarios
                .Select(u => MapearUsuario(u, u.Rol.Nombre))
                .ToList();

            return ResultadoServicio<List<UsuarioRespuestaDto>>.Ok(
                resultado,
                "Usuarios consultados correctamente.");
        }

        // ==========================================
        // CONSULTAR USUARIO POR ID
        // ==========================================

        public async Task<ResultadoServicio<UsuarioRespuestaDto>>
            ObtenerPorIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "El usuario solicitado no existe.",
                    404);
            }

            return ResultadoServicio<UsuarioRespuestaDto>.Ok(
                MapearUsuario(usuario, usuario.Rol.Nombre));
        }

        // ==========================================
        // ACTUALIZAR USUARIO
        // ==========================================

        public async Task<ResultadoServicio<UsuarioRespuestaDto>>
            ActualizarAsync(int id, ActualizarUsuarioDto dto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "El usuario que desea actualizar no existe.",
                    404);
            }

            string correo = dto.Correo.Trim().ToLowerInvariant();

            bool correoExiste = await _context.Usuarios
                .AnyAsync(u => u.Correo == correo && u.Id != id);

            if (correoExiste)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "El correo ya está registrado por otro usuario.",
                    409);
            }

            var rol = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == dto.RolId && r.Activo);

            if (rol == null)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "El rol seleccionado no existe o está inactivo.",
                    400);
            }

            usuario.Nombre = dto.Nombre.Trim();
            usuario.Apellido = dto.Apellido.Trim();
            usuario.Correo = correo;
            usuario.RolId = dto.RolId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return ResultadoServicio<UsuarioRespuestaDto>.Error(
                    "No se pudo actualizar el usuario. Verifique que el correo no esté duplicado.",
                    409);
            }

            return ResultadoServicio<UsuarioRespuestaDto>.Ok(
                MapearUsuario(usuario, rol.Nombre),
                "Usuario actualizado correctamente.");
        }

        // ==========================================
        // DESACTIVAR USUARIO
        // ==========================================

        public async Task<ResultadoServicio<bool>> DesactivarAsync(int id)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return ResultadoServicio<bool>.Error(
                    "El usuario que desea desactivar no existe.",
                    404);
            }

            if (!usuario.Activo)
            {
                return ResultadoServicio<bool>.Error(
                    "El usuario ya se encuentra desactivado.",
                    409);
            }

            usuario.Activo = false;

            await _context.SaveChangesAsync();

            return ResultadoServicio<bool>.Ok(
                true,
                "Usuario desactivado correctamente.");
        }

        // ==========================================
        // MAPEAR ENTIDAD A DTO
        // ==========================================

        private static UsuarioRespuestaDto MapearUsuario(
            Usuario usuario,
            string rolNombre)
        {
            return new UsuarioRespuestaDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                RolNombre = rolNombre,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            };
        }
    }
}