using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models.Helpers
{
    public class AuditTable
    {
        public int IdUsuarioAlta { get; set; }
        public DateTime FechaAlta { get; set; }
        public int? IdUsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? IdUsuarioBaja { get; set; }
        public DateTime? FechaBaja { get; set; }
    }
}
