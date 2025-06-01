using System;

namespace LicitAR.Web.Models
{
    public class AuditTrailViewModel
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime FechaHora { get; set; }
        public string Accion { get; set; }
        public string Entidad { get; set; }
        public int? EntidadId { get; set; }
        public string Descripcion { get; set; }
        public string IpCliente { get; set; }
        public string UserAgent { get; set; }
        public string UsuarioNombre { get; set; } // Opcional, para mostrar nombre de usuario
    }
}
