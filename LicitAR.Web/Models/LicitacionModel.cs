using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Data; // Ensure you have access to the DbContext or service
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using NuGet.Packaging.Licenses;
using Microsoft.Extensions.Azure;

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
         
        [Display(Name = "Fecha de Publicación")]
        public DateTime? FechaPublicacion { get; set; }
         
        [Display(Name = "Fecha de Cierre")]
        public DateTime? FechaCierre { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Estado")]
        public int IdEstadoLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [Display(Name = "Categoría Licitación")]
        public int IdCategoriaLicitacion { get; set; }



        public List<LicitacionDetalleModel> Items { get; set; } = new List<LicitacionDetalleModel>();

        public Licitacion GetLicitacion(AuditTable audit, EstadoLicitacion estadoLicitacion, CategoriaLicitacion categoriaLicitacion)
        {
            if (estadoLicitacion == null)
            {
                throw new InvalidOperationException($"EstadoLicitacion with ID {this.IdEstadoLicitacion} not found.");
            }
            if (categoriaLicitacion == null)
            {
                throw new InvalidOperationException($"CategoriaLicitacion with ID {this.IdCategoriaLicitacion} not found.");
            }

            List<LicitacionDetalle> detalle = new List<LicitacionDetalle>();
            int nroItem = 0;
            foreach(var detallin in this.Items)
            {
                LicitacionDetalle res = new LicitacionDetalle
                {
                    Audit = null,
                    Cantidad = detallin.Cantidad,
                    Descripcion = detallin.Descripcion,
                    IdLicitacion = detallin.IdLicitacion,
                    IdLicitacionDetalle = detallin.IdLicitacionDetalle,
                    Item = detallin.Item,
                    Licitacion = null,
                    NroItem = detallin.NroItem,
                    PrecioEstimadoUnitario = detallin.PrecioEstimadoUnitario
                };

                if(detallin.Eliminado == true)
                {
                    res.Audit = AuditHelper.SetDeletionData(AuditHelper.GetCreationData(audit.IdUsuarioAlta), audit.IdUsuarioAlta);

                }else
                {
                    res.Audit = AuditHelper.GetCreationData(audit.IdUsuarioAlta);
                    res.NroItem = nroItem;
                    nroItem++;
                }
                if (res.Item != null)
                    detalle.Add(res);
            }


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
                Titulo = this.Titulo,
                EstadoLicitacion = estadoLicitacion,
                CategoriaLicitacion = categoriaLicitacion, // Pass the provided CategoriaLicitacion object
                Items = detalle
            };
        }

        public List<LicitacionDetalle> GetLicitacionDetalles(AuditTable audit)
        {
            List<LicitacionDetalle> lista = new List<LicitacionDetalle>();
            foreach (var detalle in this.Items)
            {
                lista.Add(detalle.GetLicitacionDetalle(audit, null));
            }

            return lista;
        }

        public void SetLicitacionData(Licitacion licitacion)
        {
            this.CodigoLicitacion = licitacion.CodigoLicitacion;
            this.Descripcion = licitacion.Descripcion;
            this.FechaCierre = licitacion.FechaCierre;
            this.FechaPublicacion = licitacion.FechaPublicacion;
            this.IdCategoriaLicitacion = licitacion.IdCategoriaLicitacion;
            this.IdEntidadLicitante = licitacion.IdEntidadLicitante;
            this.IdEstadoLicitacion = licitacion.IdEstadoLicitacion;
            this.IdLicitacion = licitacion.IdLicitacion;
            this.Titulo = licitacion.Titulo;
            this.Items =licitacion.Items.Where(x=> x.Audit.FechaBaja == null).Select(x => new LicitacionDetalleModel
            {
                Cantidad = x.Cantidad,
                Descripcion = x.Descripcion,
                Eliminado = false,
                IdLicitacion = x.IdLicitacion,
                IdLicitacionDetalle = x.IdLicitacionDetalle ,
                Item = x.Item,
                NroItem = x.NroItem,
                PrecioEstimadoUnitario   = x.PrecioEstimadoUnitario
            }).ToList();
        }


    }

    public class LicitacionDetalleModel
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacionDetalle { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int NroItem { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string Item { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int Cantidad { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal PrecioEstimadoUnitario { get; set; }
        public bool Eliminado { get; set; }

        public LicitacionDetalle GetLicitacionDetalle(AuditTable audit, Licitacion? licitacion = null)
        {
            return new LicitacionDetalle
            {
                Audit = audit,
                IdLicitacion = this.IdLicitacion,
                IdLicitacionDetalle = this.IdLicitacionDetalle,
                NroItem = this.NroItem,
                Cantidad = this.Cantidad,
                Descripcion = this.Descripcion,
                Item = this.Item,
                Licitacion = licitacion,
                PrecioEstimadoUnitario = this.PrecioEstimadoUnitario
            };
        }

        public void SetLicitacionDetalleData (LicitacionDetalle licitacionDetalle)
        {
            this.IdLicitacionDetalle = licitacionDetalle.IdLicitacionDetalle;
            this.IdLicitacion = licitacionDetalle.IdLicitacion;
            this.NroItem = licitacionDetalle.NroItem;
            this.Item = licitacionDetalle.Item;
            this.Descripcion = licitacionDetalle.Descripcion;
            this.Cantidad = licitacionDetalle.Cantidad;
            this.PrecioEstimadoUnitario = licitacionDetalle.PrecioEstimadoUnitario;
        }
    }

    public class LicitacionPublicarConfirmModel
    {
        public int IdLicitacion { get; set; }
        public DateTime FechaCierre { get; set; }
    }
}
