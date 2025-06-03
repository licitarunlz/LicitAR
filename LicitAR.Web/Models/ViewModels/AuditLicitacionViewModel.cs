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
        public string UsuarioMail { get; set; } // Nuevo: mail del usuario
        public string UsuarioNombreCompleto { get; set; } // Nuevo: nombre + apellido
        public string LicitacionTitulo { get; set; }
    }
}
