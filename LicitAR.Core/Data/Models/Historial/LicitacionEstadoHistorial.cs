using System;
using LicitAR.Core.Data.Models.Parametros;

namespace LicitAR.Core.Data.Models.Historial
{
    public class LicitacionEstadoHistorial
    {
        public int Id { get; set; }
        public int IdLicitacion { get; set; }
        public int? IdEstadoAnterior { get; set; }
        public int IdEstadoNuevo { get; set; }
        public DateTime FechaCambio { get; set; }
        public int IdUsuarioCambio { get; set; }
        public string? Observaciones { get; set; }

        public virtual Licitacion Licitacion { get; set; }
        public virtual EstadoLicitacion EstadoNuevo { get; set; }
        public virtual EstadoLicitacion? EstadoAnterior { get; set; }
    }
}
