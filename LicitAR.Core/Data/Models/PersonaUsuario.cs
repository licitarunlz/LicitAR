using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Identidad;
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

    [PrimaryKey(nameof(IdPersona), nameof(IdUsuario))]
    public class PersonaUsuario
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public Persona Persona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(450, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public LicitArUser Usuario { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
