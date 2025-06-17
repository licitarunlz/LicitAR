using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models
{
    [PrimaryKey("IdOferta")]
    public class Oferta
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOferta { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required int IdLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required int IdPersona { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Fecha de Oferta")]
        public required DateTime FechaOferta { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required int IdEstadoOferta { get; set; }

        [Display(Name = "Cód. Oferta")]
        [MaxLength(20, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? CodigoOfertaLicitacion { get; set; }

        [ForeignKey("IdEstadoOferta")]
        public EstadoOferta? EstadoOferta { get; set; }

        [ForeignKey("IdLicitacion")]
        public Licitacion? Licitacion { get; set; }
        // Navegación hacia los ítems (detalle)
        public virtual ICollection<OfertaDetalle> Items { get; set; } = new List<OfertaDetalle>();


        [ForeignKey("IdPersona")]
        public Persona? Persona { get; set; }
        public required AuditTable Audit { get; set; }

    }
}
