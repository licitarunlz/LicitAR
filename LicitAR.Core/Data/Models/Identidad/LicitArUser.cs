using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.AspNetCore.Identity;

namespace LicitAR.Core.Data.Models.Identidad
{
    public class LicitArUser : IdentityUser
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Apellido { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(20, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Cuit { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; } // autonumérico

        public AuditTable Audit { get; set; }

        // Nuevo campo Enabled (eliminacion logica)
        public bool Enabled { get; set; } = true; // Por defecto, habilitado

        public ICollection<EntidadLicitanteUsuario> EntidadesLicitantes { get; set; }
    }
}