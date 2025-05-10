using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models
{
    [PrimaryKey("IdEntidadLicitante")]
    public class EntidadLicitante : WithDireccion
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEntidadLicitante { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Cuit { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string RazonSocial { get; set; }

        [NotMapped]
        public required AuditTable Audit { get; set; }

        public ICollection<EntidadLicitanteUsuario> Usuarios { get; set; }
    }


    [PrimaryKey("IdContacto")]
    public class ContactoEntidadLicitante
    {

        public int IdContactoEntidadLicitante { get; set; }
        public int IdEntidadLicitante { get; set; }
        public int IdTipoContacto { get; set; }

        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]

        public string? Email { get; set; }
        public string? CodigoArea { get; set; }
        public string? Numero { get; set; }

        [NotMapped]
        public required AuditTable Audit { get; set; }

    }
}
