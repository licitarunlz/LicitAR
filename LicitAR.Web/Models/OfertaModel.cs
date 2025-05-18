using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Web.Models
{
    public class OfertaModel
    {
        public int IdOferta { get; set; } 
        public required int IdLicitacion { get; set; }
        public Licitacion? Licitacion { get; set; }
         
        public required int IdPersona { get; set; }
         
        public required DateTime FechaOferta { get; set; }
         
        public required int IdEstadoOferta { get; set; }
    }
}
