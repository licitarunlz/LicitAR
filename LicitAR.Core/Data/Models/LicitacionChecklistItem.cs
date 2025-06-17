using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models
{

    [PrimaryKey("IdLicitacionChecklistItem")]
    public class LicitacionChecklistItem
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacionChecklistItem { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacion { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string DescripcionItem { get; set; }
        public bool DocumentoObligatorio { get; set; }

        [ForeignKey("IdLicitacion")]
        // Navegación hacia la cabecera
        public virtual Licitacion Licitacion { get; set; }
        public ICollection<OfertaChecklistItem> OfertasChecklistItems { get; set; } = new List<OfertaChecklistItem>();

        public required AuditTable Audit { get; set; }

    }
}
