using System.Security.Principal;

namespace Api.Models
{
    public class Prestamo
    {
        public int Identificacion { get; set; }
        public int Monto { get; set; }
        public int Plazo { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; }
    }
}
