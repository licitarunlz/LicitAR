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
     
    [PrimaryKey("IdEntidadLicitante")]
    public class Licitacion
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)] 
        public required int IdEntidadLicitante { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string? CodigoLicitacion { get; set; }
        
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required string Titulo { get; set; }


        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(2000, ErrorMessage =  ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required DateTime FechaPublicacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required DateTime FechaCierre { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required int IdEstadoLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public required int IdCategoriaLicitacion { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
