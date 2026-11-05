namespace Lecheria.Application.Exception;

/// <summary>Errores de aplicacion/validacion que se traducen en respuestas HTTP 400.</summary>
public class AppException : System.Exception
{
    public AppException(string mensaje) : base(mensaje) { }
}
