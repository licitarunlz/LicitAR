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
     
    [PrimaryKey("IdLicitacion")]
    public class Licitacion
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)] 
        public required int IdEntidadLicitante { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(20, ErrorMessage = ErrorMessages.MAXLENGTH)]
        [Display(Name = "Cód. Licitación")]
        public string? CodigoLicitacion { get; set; }
        
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]
        [Display(Name = "Título")]
        public required string Titulo { get; set; }


        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(2000, ErrorMessage =  ErrorMessages.MAXLENGTH)]
        [Display(Name = "Descripción")]
        public required string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Fecha de Publicación")]
        public required DateTime FechaPublicacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Fecha de Cierre")]
        public required DateTime FechaCierre { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Estado")]
        public required int IdEstadoLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Categoría Licitación")]
        public required int IdCategoriaLicitacion { get; set; }

        public required AuditTable Audit { get; set; }
    }
}
