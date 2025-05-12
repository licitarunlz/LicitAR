using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models.Parametros
{
    [PrimaryKey("IdProvincia")]
    [Table("Provincias")]
    public class Provincia
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdProvincia { get; set; }

        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }

        public required AuditTable Audit { get; set; }
        
        public bool Enabled { get; set; } = true; // Por defecto, habilitado

    }
}
