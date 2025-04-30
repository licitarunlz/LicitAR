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
            public int IdEntidadLicitante { get; set; }

            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [MaxLength(20, ErrorMessage = ErrorMessages.MAXLENGTH)]
            [Display(Name = "Cód. Licitación")]
            public string? CodigoLicitacion { get; set; }

            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]
            [Display(Name = "Título")]
            public string Titulo { get; set; }


            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [MaxLength(2000, ErrorMessage = ErrorMessages.MAXLENGTH)]
            [Display(Name = "Descripción")]
            public string Descripcion { get; set; }

            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [Display(Name = "Fecha de Publicación")]
            public DateTime FechaPublicacion { get; set; }

            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [Display(Name = "Fecha de Cierre")]
            public DateTime FechaCierre { get; set; }

            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [Display(Name = "Estado")]
            public int IdEstadoLicitacion { get; set; }

            [Required(ErrorMessage = ErrorMessages.REQUIRED)]
            [Display(Name = "Categoría Licitación")]
            public int IdCategoriaLicitacion { get; set; }


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

        public void SetLicitacionData(Licitacion licitacion)
        {
            this.CodigoLicitacion = licitacion.CodigoLicitacion;
            this.Descripcion = licitacion.Descripcion;
            this.FechaCierre = licitacion.FechaCierre;
            this.FechaPublicacion = licitacion.FechaPublicacion;
            this.IdCategoriaLicitacion = licitacion.IdCategoriaLicitacion;
            this.IdEntidadLicitante = licitacion.IdEntidadLicitante;
            this.IdLicitacion = licitacion.IdLicitacion;
            this.Titulo = licitacion.Titulo;
        }

    }
}
