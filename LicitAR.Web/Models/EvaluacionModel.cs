using LicitAR.Core.Data.Models;

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
        public Dictionary<int, int> Evaluaciones { get; set; } = new Dictionary<int, int>();

        public Evaluacion GetEvaluacion()
        {
            return new Evaluacion
            {
                EvaluacionOfertasDetalles = null,
                EvaluacionOfertas = null,
                EstadoEvaluacion = null,
                Audit = null,
                FechaFinEvaluacion = this.FechaFinEvaluacion,
                FechaInicioEvaluacion = this.FechaInicioEvaluacion.Value,
                IdEstadoEvaluacion = this.IdEstadoEvaluacion,
                IdEvaluacion = this.IdEvaluacion,
                IdLicitacion = this.IdLicitacion,
                IdUsuarioEvaluador = this.IdUsuarioEvaluador,
                Licitacion = null,

            };
        }
    }
}
