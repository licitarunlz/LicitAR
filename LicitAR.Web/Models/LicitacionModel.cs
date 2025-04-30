using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Web.Models
{
    public class LicitacionModel
    {
          
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
            [MaxLength(2000, ErrorMessage = ErrorMessages.MAXLENGTH)]
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


        public Licitacion GetLicitacion(AuditTable audit)
        {
            return new Licitacion
            {
                CodigoLicitacion = this.CodigoLicitacion,
                Audit = audit,
                Descripcion = this.Descripcion,
                FechaCierre = this.FechaCierre,
                FechaPublicacion = this.FechaPublicacion,
                IdCategoriaLicitacion = this.IdCategoriaLicitacion,
                IdEntidadLicitante = this.IdEntidadLicitante,
                IdEstadoLicitacion = this.IdEstadoLicitacion,
                Titulo = this.Titulo
            };
        }

    }
}
