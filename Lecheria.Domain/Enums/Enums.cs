namespace Lecheria.Domain.Enums;

/// <summary>Sexo del animal.</summary>
public enum Sexo
{
    Hembra = 1,
    Macho = 2
}

/// <summary>Estado productivo/sanitario general de la res.</summary>
public enum EstadoRes
{
    Activa = 1,
    Vendida = 2,
    Baja = 3,
    EnTratamiento = 4
}

/// <summary>Tipo de evento sanitario (HU-08).</summary>
public enum TipoEventoSanitario
{
    Vacuna = 1,
    Tratamiento = 2,
    Observacion = 3
}

/// <summary>Jornada del ordeno (HU-14).</summary>
public enum Jornada
{
    Manana = 1,
    Tarde = 2,
    Noche = 3
}
