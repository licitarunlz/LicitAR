using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models.Parametros
{
    [PrimaryKey("IdRubro")]
    public class Rubro
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdRubro { get; set; }
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Descripcion { get; set; }
        public required AuditTable Audit { get; set; }

    }
}
