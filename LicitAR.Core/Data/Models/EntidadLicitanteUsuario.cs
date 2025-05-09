using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Identidad; // Add this line to resolve LicitArUser
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models
{
    public class EntidadLicitanteUsuario
    {
        public int IdEntidadLicitante { get; set; }
        public EntidadLicitante EntidadLicitante { get; set; }

        public string IdUsuario { get; set; }
        public LicitArUser Usuario { get; set; }

        [NotMapped]
        public AuditTable Audit { get; set; }
    }
}
