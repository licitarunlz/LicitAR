namespace LicitAR.Web.Models
{
    public class OferenteModel
    {
        public int Id { get; set; }
        public int IdLicitacion { get; set; } // Nuevo campo
        public string TituloLicitacion { get; set; } // Nuevo campo
        public string RequisitosLicitacion { get; set; } // Nuevo campo
        public string NombreEntidad { get; set; }
        public DateTime Fecha { get; set; }
        public bool CumpleRequisitos { get; set; }
    }
}
