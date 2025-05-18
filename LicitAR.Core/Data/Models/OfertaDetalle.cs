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
    [PrimaryKey("IdOfertaDetalle")]
    public class OfertaDetalle
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOfertaDetalle { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOferta {  get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacionDetalle { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public decimal ImporteUnitario { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public decimal ImporteSubtotal { get; set; }

        public Oferta? Oferta { get; set; }
        public LicitacionDetalle? LicitacionDetalle { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
