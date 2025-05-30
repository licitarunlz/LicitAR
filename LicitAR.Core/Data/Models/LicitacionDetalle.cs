using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models
{

    [PrimaryKey("IdLicitacionDetalle")]
    public class LicitacionDetalle
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacionDetalle { get; set; }
         
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacion { get; set; }
        public int NroItem { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string Item { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int Cantidad { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioEstimadoUnitario { get; set; }
 
        // Navegación hacia la cabecera
        public virtual Licitacion Licitacion { get; set; }
        public ICollection<OfertaDetalle> OfertasDetalle { get; set; } = new List<OfertaDetalle>();

        public required AuditTable Audit { get; set; }

    }
}
