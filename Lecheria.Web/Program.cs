using Lecheria.Application.Interfaces;
using Lecheria.Application.Service;
using Lecheria.Domain.Entities;
using Lecheria.Infrastructure;
using Lecheria.Infrastructure.Persistence;
using Lecheria.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// CONTROLADORES Y SWAGGER
// ==========================================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "LESAN API - Sistema de Gestión Ganadera",
        Version = "v1",
        Description =
            "API REST para la gestión ganadera de " +
            "la Lechería San Antonio (LESAN)."
    });

    // ======================================
    // AUTENTICACIÓN JWT EN SWAGGER
    // ======================================

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "Introduce el token JWT obtenido al iniciar sesión.",

        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ==========================================
// BASE DE DATOS LESAN
// ==========================================

// Obtener cadena de conexión principal

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión DefaultConnection.");
}

// ==========================================
// CONTEXTO DE USUARIOS Y AUTENTICACIÓN
// ==========================================

// Gestiona usuarios, roles y tokens revocados

builder.Services.AddDbContext<LecheriaDbContext>(options =>
    options.UseSqlServer(connectionString));

// ==========================================
// CONTEXTO DE GESTIÓN DE RESES
// ==========================================

// Gestiona el registro y consulta de reses

builder.Services.AddDbContext<LecheriaDb>(options =>
    options.UseSqlServer(connectionString));

// ==========================================
// SERVICIOS
// ==========================================

// ======================================
// SERVICIO PARA CONTRASEÑAS
// ======================================

builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();

// ======================================
// SERVICIOS DE USUARIOS
// ======================================

builder.Services.AddScoped<
    IUsuarioService,
    UsuarioService>();

// ======================================
// SERVICIO DE AUTENTICACIÓN
// ======================================

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

// ==========================================
// SERVICIOS Y REPOSITORIOS DE RESES
// ==========================================

// Repositorio para gestionar las reses

builder.Services.AddScoped<
    IResRepository,
    ResRepositorio>();

// Servicio para registrar y consultar reses

builder.Services.AddScoped<ResService>();

// ==========================================
// CONFIGURACIÓN JWT
// ==========================================

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

if (string.IsNullOrWhiteSpace(jwtKey) ||
    string.IsNullOrWhiteSpace(jwtIssuer) ||
    string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException(
        "La configuración JWT está incompleta.");
}

byte[] jwtKeyBytes;

try
{
    jwtKeyBytes = Convert.FromBase64String(jwtKey);
}
catch (FormatException)
{
    throw new InvalidOperationException(
        "La clave JWT debe estar codificada en Base64.");
}

if (jwtKeyBytes.Length < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe contener al menos 32 bytes.");
}

// ==========================================
// AUTENTICACIÓN JWT
// ==========================================

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            IssuerSigningKey =
                new SymmetricSecurityKey(jwtKeyBytes),

            ClockSkew = TimeSpan.Zero,

            RoleClaimType = "role",

            NameClaimType = "name"
        };

    // ======================================
    // VALIDACIONES DE SEGURIDAD DEL TOKEN
    // ======================================

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            // ==================================
            // 1. OBTENER ID DEL USUARIO
            // ==================================

            var idTexto = context.Principal?
                .FindFirstValue("sub");

            if (!int.TryParse(idTexto, out int usuarioId))
            {
                context.Fail("Token de usuario inválido.");
                return;
            }

            // ==================================
            // 2. OBTENER CONEXIÓN A BASE DE DATOS
            // ==================================

            var db = context.HttpContext
                .RequestServices
                .GetRequiredService<LecheriaDbContext>();

            // ==================================
            // 3. COMPROBAR QUE EL USUARIO EXISTE
            // ==================================

            var usuario = await db.Usuarios
                .AsNoTracking()
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(
                    u => u.Id == usuarioId);

            if (usuario == null ||
                !usuario.Activo ||
                !usuario.Rol.Activo)
            {
                context.Fail(
                    "El usuario o su rol se encuentra inactivo.");

                return;
            }

            // ==================================
            // 4. VALIDAR EL ROL DEL USUARIO
            // ==================================

            var rolToken = context.Principal?
                .FindFirstValue("role");

            if (rolToken != usuario.Rol.Nombre)
            {
                context.Fail(
                    "El rol del usuario ha cambiado.");

                return;
            }

            // ==================================
            // 5. OBTENER IDENTIFICADOR DEL TOKEN
            // ==================================

            var jti = context.Principal?
                .FindFirstValue("jti");

            if (string.IsNullOrWhiteSpace(jti))
            {
                context.Fail(
                    "El token no contiene un identificador válido.");

                return;
            }

            // ==================================
            // 6. VERIFICAR SI EL TOKEN FUE REVOCADO
            // ==================================

            var tokenRevocado = await db.TokensRevocados
                .AsNoTracking()
                .AnyAsync(t => t.Jti == jti);

            if (tokenRevocado)
            {
                context.Fail(
                    "Este token fue revocado. Inicia sesión nuevamente.");

                return;
            }
        }
    };
});

// ==========================================
// AUTORIZACIÓN POR ROLES
// ==========================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdministrador", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Administrador");
    });
});

// ==========================================
// CONSTRUIR APLICACIÓN
// ==========================================

var app = builder.Build();

// ==========================================
// CREAR PRIMER ADMINISTRADOR
// SOLO EN DESARROLLO
// ==========================================

if (app.Environment.IsDevelopment())
{
    var nombre = app.Configuration["BootstrapAdmin:Nombre"];
    var apellido = app.Configuration["BootstrapAdmin:Apellido"];
    var correo = app.Configuration["BootstrapAdmin:Correo"];
    var password = app.Configuration["BootstrapAdmin:Password"];

    if (!string.IsNullOrWhiteSpace(nombre) &&
        !string.IsNullOrWhiteSpace(apellido) &&
        !string.IsNullOrWhiteSpace(correo) &&
        !string.IsNullOrWhiteSpace(password))
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<LecheriaDbContext>();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<Usuario>>();

        await InicializadorAdministrador
            .CrearAdministradorAsync(
                db,
                passwordHasher,
                nombre,
                apellido,
                correo,
                password);
    }

    // ======================================
    // ACTIVAR SWAGGER EN DESARROLLO
    // ======================================

    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "LESAN API v1");

        options.RoutePrefix = "swagger";
    });
}

// ==========================================
// MIDDLEWARE
// ==========================================

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();