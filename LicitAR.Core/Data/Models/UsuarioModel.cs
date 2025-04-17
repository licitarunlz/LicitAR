using System;

namespace LicitAR.Core.Data.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Cuit { get; set; }
    }
}