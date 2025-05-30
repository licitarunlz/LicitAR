using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Web.Helpers;
using System.Runtime.InteropServices.Marshalling;

namespace LicitAR.Web.Models
{
    public class EvaluacionModel
    {
        public int IdEvaluacion { get; set; } 
        public int IdLicitacion { get; set; }
        public int IdUsuarioEvaluador { get; set; } 
        public int IdEstadoEvaluacion { get; set; }
        public DateTime? FechaInicioEvaluacion { get; set; } 
        public DateTime? FechaFinEvaluacion { get; set; }
        public Dictionary<int, int> Ofertas { get; set; } = new Dictionary<int, int>();

        public Evaluacion GetEvaluacion(AuditTable audit)
        {
            return new Evaluacion
            {
                FechaFinEvaluacion = this.FechaFinEvaluacion,
                FechaInicioEvaluacion = this.FechaInicioEvaluacion.Value,
                IdEstadoEvaluacion = this.IdEstadoEvaluacion,
                IdEvaluacion = this.IdEvaluacion,
                IdLicitacion = this.IdLicitacion,
                IdUsuarioEvaluador = this.IdUsuarioEvaluador,
                Audit = audit,
                EvaluacionOfertasDetalles = null,
                EvaluacionOfertas = null,
                EstadoEvaluacion = null,
                Licitacion = null,

            };
        }

        public IEnumerable<EvaluacionOfertaDetalle> GetEvaluacionOferta(AuditTable audit)
        {
            List<EvaluacionOfertaDetalle> evaluacionesOfertas = new List<EvaluacionOfertaDetalle>();
            foreach(KeyValuePair<int, int> i in this.Ofertas )
            {
                evaluacionesOfertas.Add(new EvaluacionOfertaDetalle
                {
                    IdEvaluacionOfertaDetalle = 0,
                    IdEvaluacion = this.IdEvaluacion,
                    IdOfertaDetalle = i.Value,
                    OfertaDetalleGanadora = true,
                    Audit = audit,
                    Evaluacion = null,
                    IdOferta = 0
                });
            }
            return evaluacionesOfertas;
        }
   
        public void SetEvaluacion(Evaluacion eval)
        {
            this.IdEvaluacion = eval.IdEvaluacion;
            this.IdEstadoEvaluacion = eval.IdEstadoEvaluacion;
            this.IdUsuarioEvaluador = eval.IdUsuarioEvaluador;
            this.IdLicitacion = eval.IdLicitacion;
            this.FechaInicioEvaluacion = eval.FechaInicioEvaluacion;
            this.FechaFinEvaluacion = eval.FechaFinEvaluacion;
        }
    }
}
