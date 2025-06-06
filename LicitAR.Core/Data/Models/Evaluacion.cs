﻿using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models
{
    [PrimaryKey("IdEvaluacion")]
    public class Evaluacion
    {

        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEvaluacion { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLicitacion { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdUsuarioEvaluador { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEstadoEvaluacion { get; set; }
        [Display(Name = "Estado Evaluación")]
        public EstadoEvaluacion EstadoEvaluacion { get; set; }
        public DateTime FechaInicioEvaluacion { get; set; }
        public DateTime? FechaFinEvaluacion { get; set; }
        public Licitacion Licitacion { get; set; }
        public ICollection<EvaluacionOferta> EvaluacionOfertas { get; set; }
        public ICollection<EvaluacionOfertaDetalle> EvaluacionOfertasDetalles { get; set; }

        public AuditTable Audit { get; set; }

    }

    [PrimaryKey("IdEvaluacionOferta")]
    public class EvaluacionOferta
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEvaluacionOferta { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEvaluacion { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOferta { get; set; }
        public bool OfertaGanadora { get; set; }
        public Evaluacion? Evaluacion { get; set; }
        public Oferta? Oferta { get; set; }
        public AuditTable Audit { get; set; }
    }

    [PrimaryKey("IdEvaluacionOfertaDetalle")]
    public class EvaluacionOfertaDetalle
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEvaluacionOfertaDetalle { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdEvaluacion { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOferta { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdOfertaDetalle { get; set; }
        public bool OfertaDetalleGanadora { get; set; }
        public Evaluacion? Evaluacion { get; set; }
        public EstadoEvaluacion? EstadoEvaluacion { get; set; }
        public Oferta? Oferta { get; set; }
        public OfertaDetalle? OfertaDetalle { get; set; }
        public AuditTable Audit { get; set; }
    }

}
