using System;

namespace Lecheria.Domain.Entities
{
    public class TokenRevocado
    {
        public int Id { get; set; }

        public string Jti { get; set; } = string.Empty;

        public DateTime FechaExpiracion { get; set; }

        public DateTime FechaRevocacion { get; set; } = DateTime.UtcNow;
    }
}