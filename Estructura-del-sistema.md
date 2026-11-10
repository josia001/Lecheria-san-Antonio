# Estructura del sistema ganadero

La solución organiza esta entrega de registro y consulta de ficha de res en cuatro capas. Su alcance corresponde a HU-03 y HU-04 del plan del equipo.

```text
Lecheria-san-Antonio.sln
├── API
│   └── Lecheria.Web
│       ├── Controllers
│       │   └── Api
│       │       └── ResesController.cs
│       └── Program.cs
├── CORE
│   ├── Estructura-del-sistema.md
│   ├── Lecheria.Application
│   │   ├── Dtos/Hato
│   │   │   ├── ResDtos.cs
│   │   │   └── ResResponse.cs
│   │   ├── Exception
│   │   │   ├── AppException.cs
│   │   │   └── NoEncontradoException.cs
│   │   ├── Interfaces/Hato/IResRepository.cs
│   │   ├── Objetvalue/Peso.cs
│   │   └── Service/Hato/ResService.cs
│   └── Lecheria.Domain
│       ├── Entities
│       │   ├── EntityBase.cs
│       │   └── Hato/Res.cs
│       ├── Enums.cs
│       └── Interfaces/IRepository.cs
├── INFRASTRUCTURE
│   └── Lecheria.Infrastructure
│       └── Persistencia
│           ├── LecheriaDb.cs
│           └── Repositorios
│               ├── Repositorio.cs
│               └── ResRepositorio.cs
└── TEST
    └── Lecheria.Test
        └── OperacionTests.cs
```

## Responsabilidad de cada capa

| Capa | Proyecto | Qué contiene |
| --- | --- | --- |
| API | `Lecheria.Web` | Endpoints JSON para registrar una res y consultar su ficha, configuración de servicios y Swagger. |
| CORE | `Lecheria.Application` | DTO de entrada y respuesta, validación del registro, consulta por identificador y contratos de repositorio. |
| CORE | `Lecheria.Domain` | Entidad Res, cálculo de edad, enumeraciones Sexo y EstadoRes y contrato de repositorio. |
| INFRASTRUCTURE | `Lecheria.Infrastructure` | Modelo de SQL Server y repositorios de persistencia de reses. |
| TEST | `Lecheria.Test` | Pruebas del registro y la consulta de ficha contra SQL Server. |

`Application` depende de `Domain`. `Infrastructure` implementa los contratos del núcleo. `Web` conecta los servicios del núcleo con la infraestructura al iniciar la aplicación. La conexión a la base actual conserva sus registros de SQL Server.

## Flujo de registro y consulta

1. `POST /api/reses` recibe un `CrearResRequest` JSON con `codigo`, `raza`, `sexo`, `fechaNacimiento`, `peso` y `estado`.
2. `ResService.RegistrarAsync` valida los datos y la identificación única. `ResRepositorio` guarda la res en SQL Server.
3. La API devuelve HTTP `201` y un `ResResponse` con los datos registrados, el `id` y la edad calculada en `edadAnios`.
4. `GET /api/reses/{id}` utiliza `ResService.ObtenerPorIdAsync` para devolver la ficha con HTTP `200`, o HTTP `404` si el identificador no existe.

El README contiene las direcciones locales de Swagger y los pasos de ejecución. `OperacionTests.cs` verifica ambos casos de uso con una base de pruebas aislada.
