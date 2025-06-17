using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Web.Models
{
    public class OfertaModel
    {
        public int IdOferta { get; set; } 
        public required int IdLicitacion { get; set; } 
         
        public required int IdPersona { get; set; }
        public string? CodigoOfertaLicitacion { get; set; }
         
        public required DateTime FechaOferta { get; set; }
         
        public required int IdEstadoOferta { get; set; }
        public IEnumerable<OfertaDetalleModel> Items { get; set; } = new List<OfertaDetalleModel>();

        public Oferta GetOferta(AuditTable audit)
        {

            List<OfertaDetalle> detalle = new List<OfertaDetalle>();
            int nroItem = 0;
            foreach (var detallin in this.Items)
            {
                OfertaDetalle res = detallin.GetOfertaDetalle(audit);
                if (res != null)
                    detalle.Add(res);
            }


            return new Oferta
            {
                Audit = audit,
                IdOferta = this.IdOferta,
                EstadoOferta = null,
                FechaOferta = this.FechaOferta,
                IdEstadoOferta = this.IdEstadoOferta,
                IdLicitacion = this.IdLicitacion,
                IdPersona = this.IdPersona,
                Items = detalle,
                Licitacion = null,
                Persona = null
            };
        }

        public void SetOfertaData(Oferta oferta)
        {
            this.IdOferta = oferta.IdOferta;
            this.FechaOferta = oferta.FechaOferta;
            this.IdEstadoOferta = oferta.IdEstadoOferta;
            this.IdLicitacion = oferta.IdLicitacion;
            this.IdPersona = oferta.IdPersona;
            this.Items = oferta.Items.Where(x=> x.Audit.FechaBaja == null).Select(y=> new OfertaDetalleModel
            {
                IdLicitacionDetalle = y.IdLicitacionDetalle,
                IdOferta = y.IdOferta,
                IdOfertaDetalle = y.IdOfertaDetalle,
                ImporteSubtotal = y.ImporteSubtotal,
                ImporteUnitario = y.ImporteUnitario
            });

        }

     
        public List<OfertaDetalle> GetOfertaDetalles(AuditTable audit)
        {
            List<OfertaDetalle> lista = new List<OfertaDetalle>();
            foreach (var detalle in this.Items)
            {
                lista.Add(detalle.GetOfertaDetalle(audit));
            }

            return lista;
        }
         

    }

    public class OfertaDetalleModel
    {
        public int IdOfertaDetalle { get; set; }
        public int IdOferta { get; set; } 
        public int IdLicitacionDetalle { get; set; }
        public decimal ImporteUnitario { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal ImporteSubtotal { get; set; } 
        public string? Observacion { get; set; }
        public OfertaDetalle GetOfertaDetalle(AuditTable audit, Oferta? oferta = null)
        {
            return new OfertaDetalle
            {
                Audit = audit,
                IdOferta = this.IdOferta,
                IdOfertaDetalle = this.IdOfertaDetalle,
                ImporteSubtotal = this.ImporteSubtotal,
                Oferta = oferta,
                ImporteUnitario = this.ImporteUnitario,
                IdLicitacionDetalle = this.IdLicitacionDetalle,
                LicitacionDetalle = null,
                Observacion = this.Observacion
            };
        }

        public void SetOfertaDetalleData(OfertaDetalle ofertaDetalle)
        {
            this.IdLicitacionDetalle = ofertaDetalle.IdLicitacionDetalle;
            this.IdOfertaDetalle = ofertaDetalle.IdOfertaDetalle;
            this.IdOferta = ofertaDetalle.IdOferta;
            this.ImporteSubtotal = ofertaDetalle.ImporteSubtotal;
            this.ImporteUnitario = ofertaDetalle.ImporteUnitario;
            this.Observacion = ofertaDetalle.Observacion;
        }

    }
}
