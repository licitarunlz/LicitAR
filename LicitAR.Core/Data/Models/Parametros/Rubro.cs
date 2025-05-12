using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Core.Data.Models.Parametros
{
    [PrimaryKey("IdRubro")]
    public class Rubro
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdRubro { get; set; }

        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }

        public bool Enabled { get; set; } = true; // Por defecto, habilitado

        public required AuditTable Audit { get; set; }

    }
}
