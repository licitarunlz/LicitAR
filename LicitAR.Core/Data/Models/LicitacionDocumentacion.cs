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
    [PrimaryKey("IdLicitacionDocumentacion")]
    public class LicitacionDocumentacion
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacionDocumentacion { get; set; }
        public int IdLicitacion { get; set; }
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string TituloDocumento { get; set; }
        [MaxLength(255, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string NombreArchivoOriginal { get; set; }

        [MaxLength(2000, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string BlobUri { get; set; }

        public DateTime FechaCarga { get; set; }
        [ForeignKey("IdLicitacion")]
        public Licitacion Licitacion { get; set; }
        public AuditTable Audit { get; set; }
    }
}
