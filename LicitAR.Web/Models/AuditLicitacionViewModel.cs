using System;

namespace LicitAR.Web.Models
{
    public class AuditLicitacionViewModel
    {
        public int Id { get; set; }
        public int IdLicitacion { get; set; }
        public DateTime FechaHora { get; set; }
        public int UsuarioId { get; set; }
        public string Accion { get; set; }
        public string? CampoModificado { get; set; }
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string UsuarioNombre { get; set; }
        public string LicitacionTitulo { get; set; }
    }
}
