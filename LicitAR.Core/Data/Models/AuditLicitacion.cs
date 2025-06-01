using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models
{
    public class AuditLicitacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdLicitacion { get; set; }

        [ForeignKey("IdLicitacion")]
        public Licitacion Licitacion { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Accion { get; set; }

        [MaxLength(100)]
        public string? CampoModificado { get; set; }

        [MaxLength(2000)]
        public string? ValorAnterior { get; set; }

        [MaxLength(2000)]
        public string? ValorNuevo { get; set; }
    }
}
