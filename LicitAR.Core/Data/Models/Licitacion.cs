using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


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

        [ForeignKey("IdEstadoLicitacion")]
        public required EstadoLicitacion EstadoLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Categoría Licitación")]
        public required int IdCategoriaLicitacion { get; set; }

        [ForeignKey("IdCategoriaLicitacion")]
        public required CategoriaLicitacion CategoriaLicitacion { get; set; }

        public bool Enabled { get; set; } = true; // Por defecto, habilitado

        public required AuditTable Audit { get; set; }
    }
}
