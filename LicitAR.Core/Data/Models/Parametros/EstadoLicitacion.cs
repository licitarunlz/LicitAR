using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models.Parametros
{
    [PrimaryKey("IdEstadoLicitacion")]
    public class EstadoLicitacion
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEstadoLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(20, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }

        public bool Enabled { get; set; } = true; // Por defecto, habilitado

        public required AuditTable Audit { get; set; }
    }
}
