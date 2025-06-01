using System;
using System.Collections.Generic;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Parametros; // Agrega este using para EstadoLicitacion

namespace LicitAR.Web.Models
{
    public class LicitacionDetailsViewModel
    {
        public int IdLicitacion { get; set; }
        public int IdEntidadLicitante { get; set; }
        public string? CodigoLicitacion { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public int IdEstadoLicitacion { get; set; }
        public int IdCategoriaLicitacion { get; set; }
        public string EntidadLicitanteFormateada { get; set; }
        public List<LicitacionDetalleModel> Items { get; set; }
        public EstadoLicitacion EstadoLicitacion { get; set; }
    }
}
