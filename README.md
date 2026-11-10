# Lechería San Antonio

API ASP.NET Core con SQL Server para registrar una res y consultar su ficha por identificador. Esta entrega corresponde a HU-03 y HU-04. La solución de Visual Studio conserva las capas API, CORE, INFRASTRUCTURE y TEST. El proyecto de inicio es **Lecheria.Web**.

## Ejecutar desde Visual Studio

1. Inicia SQL Server y revisa `ConnectionStrings.Lecheria` en `Lecheria.Web/appsettings.json`. La conexión local usa `.\SQLEXPRESS` y la base `LecheriaSanAntonio`.
2. Abre **Lecheria-san-Antonio.sln**, haz clic derecho en **Lecheria.Web → Establecer como proyecto de inicio** y selecciona el perfil **https**.
3. Presiona **F5** y abre Swagger en [https://localhost:7220/swagger](https://localhost:7220/swagger). El perfil **http** usa [http://localhost:5021/swagger](http://localhost:5021/swagger). Swagger se habilita en el entorno Development.

También puedes iniciar el proyecto desde la carpeta de la solución.

```powershell
dotnet run --project Lecheria.Web --launch-profile https
```

## Uso de la API

| Método y ruta | Resultado |
| --- | --- |
| `POST /api/reses` | `201` con la res creada y su dirección en el encabezado `Location`. |
| `GET /api/reses/{id}` | `200` con la ficha de la res, o `404` si no existe. |

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

Los errores devuelven JSON con `400` para datos inválidos o código duplicado, `404` para una ficha inexistente y `415` si el POST usa un tipo de contenido distinto de JSON.

## Configurar la base

Para trabajar con los registros actuales, conserva la conexión a la base existente. El proyecto utiliza la tabla de reses y conserva los datos de SQL Server.

En una instalación nueva, la aplicación inicializa el esquema en la base **LecheriaSanAntonio**. La conexión requiere los permisos correspondientes para crear la base y las tablas. También puedes indicar otra conexión mediante `ConnectionStrings__Lecheria`.

## Pruebas

`Lecheria.Test/OperacionTests.cs` comprueba el registro y la consulta de ficha contra SQL Server. Define `Lecheria__SqlServerPruebas` con una conexión a una instancia de pruebas que permita crear y eliminar bases temporales, y ejecuta `dotnet test`.
