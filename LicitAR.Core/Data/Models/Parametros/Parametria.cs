using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models.Parametros
{

    [PrimaryKey("IdParametria")]
    public class Parametria
    {

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdParametria { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(20, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Clave { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(2000, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Valor { get; set; }


        public required AuditTable Audit { get; set; }

    }
}
