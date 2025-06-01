using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicitAR.Core.Data.Models
{
    public class AuditTrail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public DateTime FechaHora { get; set; }

        [Required]
        [MaxLength(100)]
        public string Accion { get; set; }

        [Required]
        [MaxLength(100)]
        public string Entidad { get; set; }

        public int? EntidadId { get; set; }

        [MaxLength(2000)]
        public string Descripcion { get; set; }

        [MaxLength(50)]
        public string IpCliente { get; set; }

        [MaxLength(512)]
        public string UserAgent { get; set; }
    }
}
