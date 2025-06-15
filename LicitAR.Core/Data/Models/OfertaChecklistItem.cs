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
    [PrimaryKey("IdOfertaChecklistItem")]
    public class OfertaChecklistItem
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOfertaChecklistItem { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOferta { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacionChecklistItem { get; set; }


        [MaxLength(2000, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? BlobUri { get; set; }

        public Oferta? Oferta { get; set; }
        public LicitacionChecklistItem? LicitacionChecklistItem { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
