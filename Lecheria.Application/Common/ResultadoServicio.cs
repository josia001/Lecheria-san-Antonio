namespace Lecheria.Application.Common
{
    public class ResultadoServicio<T>
    {
        public bool Exitoso { get; private set; }

        public T? Datos { get; private set; }

        public string Mensaje { get; private set; } = string.Empty;

        public int CodigoEstado { get; private set; }

        private ResultadoServicio()
        {
        }

        public static ResultadoServicio<T> Ok(
            T datos,
            string mensaje = "Operación realizada correctamente.")
        {
            return new ResultadoServicio<T>
            {
                Exitoso = true,
                Datos = datos,
                Mensaje = mensaje,
                CodigoEstado = 200
            };
        }

        public static ResultadoServicio<T> Creado(
            T datos,
            string mensaje = "Registro creado correctamente.")
        {
            return new ResultadoServicio<T>
            {
                Exitoso = true,
                Datos = datos,
                Mensaje = mensaje,
                CodigoEstado = 201
            };
        }

        public static ResultadoServicio<T> Error(
            string mensaje,
            int codigoEstado = 400)
        {
            return new ResultadoServicio<T>
            {
                Exitoso = false,
                Datos = default,
                Mensaje = mensaje,
                CodigoEstado = codigoEstado
            };
        }
    }
}