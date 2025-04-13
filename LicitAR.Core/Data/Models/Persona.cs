using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models
{
    [PrimaryKey("IdPersona")]
    public class Persona : WithDireccion
    {

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string RazonSocial { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdTipoPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Cuit { get; set; }

        public required AuditTable Audit { get; set; }
        

    }

   


    public class ContactoPersona
    {

        public int IdContacto { get; set; }
        public int IdPersona { get; set; }
        public int IdTipoContacto { get; set; }

        public string? Email { get; set; }
        public string? CodigoArea { get; set; }
        public string? Numero { get; set; }

        public required AuditTable Audit { get; set; }

    }
}

