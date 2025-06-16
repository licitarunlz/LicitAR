namespace LicitAR.Web.ViewModels.Dashboard
{
    public class OferenteDashboardViewModel
    {
        public string? NombreOferente { get; set; }
        public int LicitacionesDisponibles { get; set; }
        public int LicitacionesEnCurso { get; set; }
        public int AdjudicacionesGanadas { get; set; }

        public int TotalLicitaciones { get; set; }
        public int TotalLicitacionesActivas { get; set; }
        public int TotalAdjudicaciones { get; set; }
        public int LicitacionesActivas { get; set; }
        public int Adjudicaciones { get; set; }

        public decimal PorcentajeLicitacionesActivasVsMesAnterior { get; set; }
        public decimal PorcentajeAdjudicacionesVsMesAnterior { get; set; }


        public class Licitacion
        {
            public string? Rubro { get; set; }
            public string? Titulo { get; set; }
            public decimal MontoEstimado { get; set; }
            public string? Nombre { get; set; }
        }

        public List<Licitacion> UltimasLicitaciones { get; set; } = new List<Licitacion>();

    }
}
