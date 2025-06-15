namespace LicitAR.Web.ViewModels.Dashboard
{
    public class OferenteDashboardViewModel
    {
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
    }
}
