using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Lecheria.Application.Common;
using Lecheria.Application.DTOs.Auth;
using Lecheria.Application.Interfaces;
using Lecheria.Domain.Entities;
using Lecheria.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Lecheria.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly LecheriaDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            LecheriaDbContext context,
            IPasswordHasher<Usuario> passwordHasher,
            IConfiguration configuration)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        // ==========================================
        // INICIAR SESIÓN
        // ==========================================

        public async Task<ResultadoServicio<LoginRespuestaDto>> LoginAsync(
            LoginDto dto)
        {
            string correo = dto.Correo.Trim().ToLowerInvariant();

            // Buscar usuario y su rol
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Correo == correo);

            if (usuario == null)
            {
                return CredencialesInvalidas();
            }

            // Comprobar que el usuario y su rol estén activos
            if (!usuario.Activo || !usuario.Rol.Activo)
            {
                return CredencialesInvalidas();
            }

            // Verificar contraseña
            PasswordVerificationResult verificacion;

            try
            {
                verificacion = _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.PasswordHash,
                    dto.Password);
            }
            catch (FormatException)
            {
                return CredencialesInvalidas();
            }

            if (verificacion == PasswordVerificationResult.Failed)
            {
                return CredencialesInvalidas();
            }

            // Obtener configuración JWT
            string? jwtKey = _configuration["Jwt:Key"];
            string? issuer = _configuration["Jwt:Issuer"];
            string? audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(jwtKey) ||
                string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience))
            {
                throw new InvalidOperationException(
                    "La configuración JWT está incompleta.");
            }

            byte[] keyBytes = Convert.FromBase64String(jwtKey);

            if (keyBytes.Length < 32)
            {
                throw new InvalidOperationException(
                    "La clave JWT debe tener al menos 32 bytes.");
            }

            if (!int.TryParse(
                _configuration["Jwt:ExpirationMinutes"],
                out int minutosExpiracion) ||
                minutosExpiracion <= 0)
            {
                throw new InvalidOperationException(
                    "El tiempo de expiración JWT no es válido.");
            }

            // Datos que se incluirán en el token
            var claims = new List<Claim>
            {
                new Claim("sub", usuario.Id.ToString()),
                new Claim("name", $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim("email", usuario.Correo),
                new Claim("role", usuario.Rol.Nombre),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            // Firmar token
            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            DateTime expiracion = DateTime.UtcNow
                .AddMinutes(minutosExpiracion);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiracion,
                signingCredentials: credenciales);

            string token = new JwtSecurityTokenHandler()
                .WriteToken(jwt);

            // Preparar respuesta
            var respuesta = new LoginRespuestaDto
            {
                Token = token,
                TipoToken = "Bearer",
                FechaExpiracion = expiracion,
                UsuarioId = usuario.Id,
                NombreCompleto =
                    $"{usuario.Nombre} {usuario.Apellido}",
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                RolNombre = usuario.Rol.Nombre
            };

            return ResultadoServicio<LoginRespuestaDto>.Ok(
                respuesta,
                "Inicio de sesión exitoso.");
        }

        // ==========================================
        // RESPUESTA PARA CREDENCIALES INVÁLIDAS
        // ==========================================

        private static ResultadoServicio<LoginRespuestaDto>
            CredencialesInvalidas()
        {
            return ResultadoServicio<LoginRespuestaDto>.Error(
                "Correo o contraseña incorrectos, o cuenta inactiva.",
                401);
        }
    }
}