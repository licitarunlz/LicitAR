using LicitAR.Core.Data.Models.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models
{
    public class LicitacionNotificacion
    {
        [Key]
        public int IdNotificacion { get; set; }

        public int IdPersona { get; set; }
        public int? IdUsuario { get; set; }
        public int IdLicitacion { get; set; }

        [Required]
        [MaxLength(200)]
        public string Titulo { get; set; }

        [MaxLength(2000)]
        public string Detalle { get; set; }

        [MaxLength(500)]
        public string UrlDestino { get; set; }

        public bool Important { get; set; } = false;
        public bool Read { get; set; } = false;

        public required AuditTable Audit { get; set; }

        [ForeignKey("IdLicitacion")]
        public Licitacion Licitacion { get; set; }
    }
}
