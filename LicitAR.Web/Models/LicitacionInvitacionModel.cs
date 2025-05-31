using System;

namespace LicitAR.Web.Models
{
    public class LicitacionInvitacionModel
    {
        public int IdLicitacion { get; set; }
        public string CodigoLicitacion { get; set; }
        public string TituloLicitacion { get; set; }
        public int IdPersona { get; set; }
        public string Cuit { get; set; }
        public string RazonSocial { get; set; }
        public DateTime FechaInvitacion { get; set; }
    }
}
