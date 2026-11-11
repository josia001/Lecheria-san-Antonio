namespace Lecheria.Application.Exception;


public class NoEncontradoException : System.Exception
{
    public NoEncontradoException(string mensaje) : base(mensaje) { }
}
