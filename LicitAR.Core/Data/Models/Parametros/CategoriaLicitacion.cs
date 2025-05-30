﻿using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Core.Data.Models.Parametros
{
    public class CategoriaLicitacion
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdCategoriaLicitacion { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }

        public bool Enabled { get; set; } = true; // Por defecto, habilitado

        public required AuditTable Audit { get; set; }
    }
}
