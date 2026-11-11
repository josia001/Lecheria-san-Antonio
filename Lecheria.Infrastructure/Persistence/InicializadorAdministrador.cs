using System;
using System.Linq;
using System.Threading.Tasks;
using Lecheria.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure.Persistence
{
    public static class InicializadorAdministrador
    {
        public static async Task CrearAdministradorAsync(
            LecheriaDbContext contexto,
            IPasswordHasher<Usuario> passwordHasher,
            string nombre,
            string apellido,
            string correo,
            string password)
        {
            // Verificar si ya existe un administrador
            bool existeAdministrador = await contexto.Usuarios
                .AnyAsync(u => u.RolId == 1);

            if (existeAdministrador)
            {
                return;
            }

            // Validar los datos recibidos
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(correo) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "Los datos del administrador están incompletos.");
            }

            if (password.Length < 12)
            {
                throw new InvalidOperationException(
                    "La contraseña del administrador debe tener al menos 12 caracteres.");
            }

            correo = correo.Trim().ToLowerInvariant();

            // Evitar duplicar el correo
            bool existeCorreo = await contexto.Usuarios
                .AnyAsync(u => u.Correo == correo);

            if (existeCorreo)
            {
                throw new InvalidOperationException(
                    "El correo configurado ya pertenece a otro usuario.");
            }

            // Verificar que exista el rol Administrador
            var rolAdministrador = await contexto.Roles
                .FirstOrDefaultAsync(r =>
                    r.Id == 1 &&
                    r.Nombre == "Administrador" &&
                    r.Activo);

            if (rolAdministrador == null)
            {
                throw new InvalidOperationException(
                    "No existe un rol Administrador activo en la base de datos.");
            }

            // Crear el administrador
            var administrador = new Usuario
            {
                Nombre = nombre.Trim(),
                Apellido = apellido.Trim(),
                Correo = correo,
                RolId = rolAdministrador.Id,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            // Proteger la contraseña
            administrador.PasswordHash =
                passwordHasher.HashPassword(administrador, password);

            // Guardar en SQL Server
            contexto.Usuarios.Add(administrador);

            await contexto.SaveChangesAsync();
        }
    }
}