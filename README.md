# Lechería San Antonio

API ASP.NET Core con SQL Server para registrar reses, consultar y actualizar sus fichas, y registrar eventos sanitarios. Incluye HU-03, HU-04, HU-05 y HU-08 junto con la integración de usuarios y autenticación del equipo. El proyecto de inicio es **Lecheria.Web**.

## Ejecutar desde Visual Studio

1. Inicia SQL Server y revisa `ConnectionStrings.DefaultConnection` en `Lecheria.Web/appsettings.json`. La configuración publicada usa LocalDB y la base `LecheriaSanAntonioDB`. Si ya usas una base de ganado separada, configura `ConnectionStrings:Lecheria` con su conexión.
2. Abre **Lecheria-san-Antonio.sln**, haz clic derecho en **Lecheria.Web → Establecer como proyecto de inicio** y selecciona el perfil **https**.
3. Presiona **F5** y abre Swagger en [https://localhost:7220/swagger](https://localhost:7220/swagger). El perfil **http** usa [http://localhost:5021/swagger](http://localhost:5021/swagger). Swagger se habilita en el entorno Development.

También puedes iniciar el proyecto desde la carpeta de la solución.

```powershell
dotnet run --project Lecheria.Web --launch-profile https
```

Configura `Jwt:Key` en los secretos de usuario de `Lecheria.Web` con una clave Base64 de al menos 32 bytes. `Jwt:Issuer` y `Jwt:Audience` están en la configuración del proyecto. La autenticación utiliza las migraciones de `LecheriaDbContext` y los usuarios existentes del equipo.

## Uso de la API

| Método y ruta | Resultado |
| --- | --- |
| `POST /api/reses` | `201` con la res creada y su dirección en el encabezado `Location`. |
| `GET /api/reses/{id}` | `200` con la ficha de la res, o `404` si no existe. |
| `PUT /api/reses/{id}` | `200` con la ficha actualizada, `400` para datos inválidos y `404` si la res no existe. |
| `POST /api/reses/{resId}/eventos-sanitarios` | `201` con el evento registrado y su dirección en `Location`. |
| `GET /api/reses/{resId}/eventos-sanitarios/{id}` | `200` con un evento de esa res, o `404` si no existe. |

Para registrar una res en Swagger o Postman, envía `POST /api/reses` con `Content-Type: application/json` y sus datos.

```json
{
  "codigo": "RES-EJEMPLO-001",
  "raza": "Holstein",
  "sexo": "Hembra",
  "fechaNacimiento": "2024-01-15",
  "peso": 420,
  "estado": "Activa"
}
```

El código debe ser único y tener un máximo de 120 caracteres tras quitar espacios de los extremos. La raza es obligatoria, el nacimiento debe ser válido y el peso debe ser positivo. Swagger muestra los valores admitidos para `sexo` y `estado`.

La respuesta contiene `id`, `codigo`, `raza`, `sexo`, `fechaNacimiento`, `edadAnios`, `peso` y `estado`. Utiliza el `id` devuelto para consultar `GET /api/reses/{id}`. La edad se calcula a partir de la fecha de nacimiento.

Para actualizar la ficha, envía a `PUT /api/reses/{id}` los seis campos del registro con sus valores completos. La actualización conserva el identificador y valida que el código pertenezca únicamente a esa res.

Para registrar un evento sanitario, inicia sesión con un usuario de rol `Veterinario` o `Administrador` y coloca su token en **Authorize** de Swagger. Envía el identificador de la res en la ruta y el siguiente JSON.

```json
{
  "tipo": "Vacuna",
  "fecha": "2026-11-18",
  "descripcion": "Vacunación registrada por el veterinario."
}
```

Los tipos admitidos son `Vacuna`, `Tratamiento` y `Observacion`. Fecha, tipo y descripción son obligatorios. La respuesta incluye `id`, `resId`, `tipo`, `fecha` y `descripcion`. Utiliza el enlace `Location` para consultar el evento guardado. Las rutas sanitarias devuelven `401` si falta autenticación y `403` si el usuario carece del rol requerido.

Los errores devuelven JSON con `400` para datos inválidos o código duplicado, `404` para una ficha inexistente y `415` si el POST usa un tipo de contenido distinto de JSON.

## Configurar la base

Para trabajar con los registros actuales, conserva la conexión a la base existente. El proyecto utiliza la tabla de reses y conserva los datos de SQL Server.

Con `DefaultConnection`, el arranque aplica las migraciones de `LecheriaDb`, incluida la tabla `EventoSanitario`. Las migraciones de usuarios y roles pertenecen a `LecheriaDbContext` y se administran por separado. Si utilizas la conexión heredada `Lecheria`, el arranque agrega la tabla sanitaria a la base de ganado existente. La cuenta SQL necesita permisos para esos cambios de esquema.

## Pruebas

`Lecheria.Test/OperacionTests.cs` comprueba el registro, la consulta y la persistencia de eventos sanitarios, incluida la ampliación de una base anterior. Define `Lecheria__SqlServerPruebas` con una conexión a una instancia de pruebas que permita crear y eliminar bases temporales, y ejecuta `dotnet test`.
