using Lecheria.Application.Dtos;
using Lecheria.Application.Exception;
using Lecheria.Application.Service;
using Lecheria.Domain.Entities;
using Lecheria.Domain.Enums;
using Lecheria.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Lecheria.Test;

public class OperacionTests
{
    [Fact]
    public async Task Registrar_persiste_la_ficha_completa_y_normaliza_los_textos()
    {
        await using var basePrueba = await BasePrueba.CrearAsync();
        var servicio = new ResService(new ResRepositorio(basePrueba.Db));
        var solicitud = SolicitudValida();
        solicitud.Codigo = " RES-001 ";
        solicitud.Raza = " Holstein ";
        var registrada = await servicio.RegistrarAsync(solicitud);

        await using var otraConexion = new LecheriaDb(basePrueba.Opciones);
        var consulta = new ResService(new ResRepositorio(otraConexion));
        var ficha = await consulta.ObtenerPorIdAsync(registrada.Id);

        Assert.True(ficha.Id > 0);
        Assert.Equal("RES-001", ficha.Codigo);
        Assert.Equal("Holstein", ficha.Raza);
        Assert.Equal(Sexo.Hembra, ficha.Sexo);
        Assert.Equal(solicitud.FechaNacimiento, ficha.FechaNacimiento);
        Assert.Equal(450.125m, ficha.Peso);
        Assert.Equal(EstadoRes.Activa, ficha.Estado);
        Assert.Equal(3, ficha.EdadAnios);
        Assert.Equal(1, await otraConexion.Set<Res>().CountAsync());
    }

    [Theory]
    [InlineData("RES-001")]
    [InlineData("res-001")]
    [InlineData(" RES-001 ")]
    public async Task Registrar_rechaza_codigos_duplicados_sin_agregar_otra_res(string codigo)
    {
        await using var basePrueba = await BasePrueba.CrearAsync();
        var servicio = new ResService(new ResRepositorio(basePrueba.Db));
        var original = await servicio.RegistrarAsync(SolicitudValida());
        var repetida = SolicitudValida();
        repetida.Codigo = codigo;

        await Assert.ThrowsAsync<AppException>(() => servicio.RegistrarAsync(repetida));

        Assert.Equal(1, await basePrueba.Db.Set<Res>().CountAsync());
        Assert.Equal("RES-001", (await servicio.ObtenerPorIdAsync(original.Id)).Codigo);
    }

    [Fact]
    public async Task Registrar_rechaza_datos_invalidos_sin_guardarlos()
    {
        await using var basePrueba = await BasePrueba.CrearAsync();
        var servicio = new ResService(new ResRepositorio(basePrueba.Db));
        var casos = new Dictionary<string, Action<CrearResRequest>>
        {
            ["código nulo"] = solicitud => solicitud.Codigo = null!,
            ["código vacío"] = solicitud => solicitud.Codigo = " ",
            ["código demasiado largo"] = solicitud => solicitud.Codigo = new string('X', Res.LongitudMaximaCodigo + 1),
            ["raza nula"] = solicitud => solicitud.Raza = null!,
            ["raza vacía"] = solicitud => solicitud.Raza = " ",
            ["sexo sin seleccionar"] = solicitud => solicitud.Sexo = 0,
            ["sexo desconocido"] = solicitud => solicitud.Sexo = (Sexo)999,
            ["estado sin seleccionar"] = solicitud => solicitud.Estado = 0,
            ["estado desconocido"] = solicitud => solicitud.Estado = (EstadoRes)999,
            ["fecha sin seleccionar"] = solicitud => solicitud.FechaNacimiento = default,
            ["fecha futura"] = solicitud => solicitud.FechaNacimiento = DateOnly.FromDateTime(DateTime.Today).AddDays(1),
            ["peso cero"] = solicitud => solicitud.Peso = 0,
            ["peso negativo"] = solicitud => solicitud.Peso = -1
        };

        foreach (var (nombre, invalidar) in casos)
        {
            var solicitud = SolicitudValida();
            invalidar(solicitud);
            var error = await Record.ExceptionAsync(() => servicio.RegistrarAsync(solicitud));
            Assert.True(error is AppException, $"El caso {nombre} debe producir un error de validación.");
            Assert.Equal(0, await basePrueba.Db.Set<Res>().CountAsync());
        }
    }

    [Fact]
    public async Task Obtener_ficha_inexistente_produce_no_encontrado()
    {
        await using var basePrueba = await BasePrueba.CrearAsync();
        var servicio = new ResService(new ResRepositorio(basePrueba.Db));

        await Assert.ThrowsAsync<NoEncontradoException>(() => servicio.ObtenerPorIdAsync(1));
    }

    [Fact]
    public async Task Registrar_eventos_sanitarios_conserva_las_reses_de_una_base_anterior()
    {
        await using var basePrueba = await BasePrueba.CrearAsync(esquemaAnterior: true);
        var reses = new ResRepositorio(basePrueba.Db);
        var res = await new ResService(reses).RegistrarAsync(SolicitudValida());

        await basePrueba.Db.InicializarAsync();
        await basePrueba.Db.InicializarAsync();

        var eventos = new EventoSanitarioService(reses, new Repositorio<EventoSanitario>(basePrueba.Db));
        await using var otraConexion = new LecheriaDb(basePrueba.Opciones);
        var consulta = new EventoSanitarioService(
            new ResRepositorio(otraConexion), new Repositorio<EventoSanitario>(otraConexion));
        var fecha = new DateOnly(2026, 11, 1);

        foreach (var tipo in Enum.GetValues<TipoEventoSanitario>())
        {
            var registro = await eventos.RegistrarAsync(res.Id, new RegistrarEventoSanitarioRequest
            {
                Tipo = tipo,
                Fecha = fecha,
                Descripcion = " Atención registrada "
            });

            var guardado = await consulta.ObtenerAsync(res.Id, registro.Id);
            Assert.Equal(tipo, guardado.Tipo);
            Assert.Equal(fecha, DateOnly.FromDateTime(guardado.Fecha));
            Assert.Equal("Atención registrada", guardado.Descripcion);
            await Assert.ThrowsAsync<NoEncontradoException>(() => consulta.ObtenerAsync(int.MaxValue, registro.Id));
        }

        var solicitud = new RegistrarEventoSanitarioRequest
        {
            Tipo = TipoEventoSanitario.Observacion,
            Fecha = fecha,
            Descripcion = "Descripción válida"
        };
        await Assert.ThrowsAsync<NoEncontradoException>(() => eventos.RegistrarAsync(int.MaxValue, solicitud));
        await Assert.ThrowsAsync<AppException>(() => eventos.RegistrarAsync(res.Id, new RegistrarEventoSanitarioRequest
        {
            Tipo = solicitud.Tipo,
            Descripcion = solicitud.Descripcion
        }));
        Assert.Equal("RES-001", (await otraConexion.Set<Res>().SingleAsync()).Codigo);
        Assert.Equal(3, await otraConexion.Set<EventoSanitario>().CountAsync());
    }

    [Fact]
    public async Task Migraciones_permiten_registrar_y_consultar_eventos_sanitarios()
    {
        await using var basePrueba = await BasePrueba.CrearAsync(aplicarMigraciones: true);
        var reses = new ResRepositorio(basePrueba.Db);
        var res = await new ResService(reses).RegistrarAsync(SolicitudValida());
        var eventos = new EventoSanitarioService(reses, new Repositorio<EventoSanitario>(basePrueba.Db));
        var evento = await eventos.RegistrarAsync(res.Id, new RegistrarEventoSanitarioRequest
        {
            Tipo = TipoEventoSanitario.Vacuna,
            Fecha = new DateOnly(2026, 11, 18),
            Descripcion = "Vacuna registrada"
        });

        await basePrueba.Db.Database.MigrateAsync();
        Assert.Empty(await basePrueba.Db.Database.GetPendingMigrationsAsync());
        Assert.False(basePrueba.Db.Database.HasPendingModelChanges());
        Assert.Equal(res.Id, (await eventos.ObtenerAsync(res.Id, evento.Id)).ResId);
    }

    private static CrearResRequest SolicitudValida() => new()
    {
        Codigo = "RES-001",
        Raza = "Holstein",
        Sexo = Sexo.Hembra,
        FechaNacimiento = DateOnly.FromDateTime(DateTime.Today).AddYears(-3),
        Peso = 450.125m,
        Estado = EstadoRes.Activa
    };

    private sealed class BasePrueba(LecheriaDb db, DbContextOptions<LecheriaDb> opciones) : IAsyncDisposable
    {
        public LecheriaDb Db => db;
        public DbContextOptions<LecheriaDb> Opciones => opciones;

        public static async Task<BasePrueba> CrearAsync(bool esquemaAnterior = false, bool aplicarMigraciones = false)
        {
            var conexion = Environment.GetEnvironmentVariable("Lecheria__SqlServerPruebas")
                ?? throw new InvalidOperationException("Configure Lecheria__SqlServerPruebas antes de ejecutar estas pruebas.");
            var builder = new SqlConnectionStringBuilder(conexion)
            {
                InitialCatalog = $"LecheriaPruebas_{Guid.NewGuid():N}",
                AttachDBFilename = string.Empty
            };
            var opciones = new DbContextOptionsBuilder<LecheriaDb>().UseSqlServer(builder.ConnectionString).Options;
            var db = new LecheriaDb(opciones);
            try
            {
                if (aplicarMigraciones)
                {
                    await db.Database.MigrateAsync();
                }
                else if (esquemaAnterior)
                {
                    await using var anterior = new LecheriaDbAnterior(opciones);
                    await anterior.Database.EnsureCreatedAsync();
                }
                else
                {
                    await db.Database.EnsureCreatedAsync();
                }
                return new BasePrueba(db, opciones);
            }
            catch
            {
                try { await db.Database.EnsureDeletedAsync(); }
                finally { await db.DisposeAsync(); }
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            try { await db.Database.EnsureDeletedAsync(); }
            finally { await db.DisposeAsync(); }
        }
    }

    private sealed class LecheriaDbAnterior(DbContextOptions<LecheriaDb> options) : LecheriaDb(options)
    {
        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);
            model.Ignore<EventoSanitario>();
        }
    }
}
