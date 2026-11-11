using System;
using Lecheria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Infrastructure.Persistence
{
    public class LecheriaDbContext : DbContext
    {
        public LecheriaDbContext(
            DbContextOptions<LecheriaDbContext> options)
            : base(options)
        {
        }

        // ==========================================
        // TABLAS DE LA BASE DE DATOS
        // ==========================================

        public DbSet<Usuario> Usuarios { get; set; } = null!;

        public DbSet<Rol> Roles { get; set; } = null!;

        // NUEVA TABLA PARA CIERRE DE SESIÓN JWT
        public DbSet<TokenRevocado> TokensRevocados { get; set; } = null!;

        // ==========================================
        // CONFIGURACIÓN DE ENTIDADES
        // ==========================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // CONFIGURACIÓN DE USUARIOS
            // ==========================================

            modelBuilder.Entity<Usuario>(entity =>
            {
                // Clave primaria
                entity.HasKey(u => u.Id);

                // Nombre
                entity.Property(u => u.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                // Apellido
                entity.Property(u => u.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                // Correo electrónico
                entity.Property(u => u.Correo)
                    .IsRequired()
                    .HasMaxLength(150);

                // Evitar correos duplicados
                entity.HasIndex(u => u.Correo)
                    .IsUnique();

                // Contraseña protegida
                entity.Property(u => u.PasswordHash)
                    .IsRequired();

                // Estado de la cuenta
                entity.Property(u => u.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Fecha de creación
                entity.Property(u => u.FechaCreacion)
                    .IsRequired();

                // Relación entre Usuario y Rol
                entity.HasOne(u => u.Rol)
                    .WithMany()
                    .HasForeignKey(u => u.RolId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // ==========================================
            // CONFIGURACIÓN DE ROLES
            // ==========================================

            modelBuilder.Entity<Rol>(entity =>
            {
                // Clave primaria
                entity.HasKey(r => r.Id);

                // Nombre del rol
                entity.Property(r => r.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                // Evitar nombres de roles duplicados
                entity.HasIndex(r => r.Nombre)
                    .IsUnique();

                // Descripción
                entity.Property(r => r.Descripcion)
                    .HasMaxLength(250);

                // Estado del rol
                entity.Property(r => r.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Fecha de creación
                entity.Property(r => r.FechaCreacion)
                    .IsRequired();

                // ==========================================
                // LOS 3 ROLES DEFINITIVOS DE LESAN
                // ==========================================

                entity.HasData(

                    // ======================================
                    // ROL 1: ADMINISTRADOR
                    // ======================================

                    new Rol
                    {
                        Id = 1,

                        Nombre = "Administrador",

                        Descripcion =
                            "Administración general del sistema, " +
                            "gestión de usuarios, ventas, costos y reportes.",

                        Activo = true,

                        FechaCreacion = new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc)
                    },

                    // ======================================
                    // ROL 2: ENCARGADO DE GANADERÍA
                    // ======================================

                    new Rol
                    {
                        Id = 2,

                        Nombre = "Encargado de Ganadería",

                        Descripcion =
                            "Gestión del ganado, alimentación, " +
                            "reproducción, ordeño y producción de leche.",

                        Activo = true,

                        FechaCreacion = new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc)
                    },

                    // ======================================
                    // ROL 3: VETERINARIO
                    // ======================================

                    new Rol
                    {
                        Id = 3,

                        Nombre = "Veterinario",

                        Descripcion =
                            "Control sanitario del ganado, " +
                            "vacunaciones, tratamientos y seguimiento veterinario.",

                        Activo = true,

                        FechaCreacion = new DateTime(
                            2026, 1, 1, 0, 0, 0,
                            DateTimeKind.Utc)
                    }
                );
            });

            // ==========================================
            // CONFIGURACIÓN DE TOKENS REVOCADOS
            // ==========================================

            modelBuilder.Entity<TokenRevocado>(entity =>
            {
                // Clave primaria
                entity.HasKey(t => t.Id);

                // Identificador único del token JWT
                entity.Property(t => t.Jti)
                    .IsRequired()
                    .HasMaxLength(100);

                // Evitar revocar dos veces el mismo token
                entity.HasIndex(t => t.Jti)
                    .IsUnique();

                // Fecha en que vence el token
                entity.Property(t => t.FechaExpiracion)
                    .IsRequired();

                // Fecha en que se revocó
                entity.Property(t => t.FechaRevocacion)
                    .IsRequired();
            });
        }
    }
}