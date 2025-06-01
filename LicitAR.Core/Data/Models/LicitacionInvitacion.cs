using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Identidad;

namespace LicitAR.Core.Data.Models
{
    [Table("LicitacionInvitacion")]
    public class LicitacionInvitacion
    {
        [Key, Column(Order = 0)]
        public int IdLicitacion { get; set; }
        public Licitacion Licitacion { get; set; }

        [Key, Column(Order = 1)]
        public int IdPersona { get; set; }
        public Persona Persona { get; set; }

        public DateTime FechaInvitacion { get; set; }

        public int? IdUsuario { get; set; }
        // Si tienes LicitArUser, puedes agregar la relación
        // public LicitArUser Usuario { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
