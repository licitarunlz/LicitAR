using System;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Core.Data.Models
{
    public class UsuarioModel
    {
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        public string Cuit { get; set; }

        public bool Enabled { get; set; } // Nuevo campo para habilitado/deshabilitado para eliminacion lógica

        public string CuitFormateado
        {
            get
            {
                if (Cuit.Length == 11)
                {
                    return $"{Cuit.Substring(0, 2)}-{Cuit.Substring(2, 8)}-{Cuit.Substring(10, 1)}";
                }
                return Cuit;
            }
        }
    }
}