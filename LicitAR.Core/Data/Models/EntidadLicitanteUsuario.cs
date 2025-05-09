using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Identidad; // Add this line to resolve LicitArUser
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Core.Data.Models
{
    [PrimaryKey(nameof(IdEntidadLicitante), nameof(IdUsuario))]
    public class EntidadLicitanteUsuario
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEntidadLicitante { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public EntidadLicitante EntidadLicitante { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public LicitArUser Usuario { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
