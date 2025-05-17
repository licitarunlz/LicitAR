using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models.Parametros
{
    public class Localidad
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLocalidad { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdProvincia { get; set; }

        [ForeignKey("IdProvincia")]
        public Provincia Provincia { get; set; } = null!;

        public bool Enabled { get; set; } = true; // Por defecto, habilitado

        public required AuditTable Audit { get; set; }
    }
}
