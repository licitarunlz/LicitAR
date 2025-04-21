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

namespace LicitAR.Core.Data.Models.Identidad
{
    [PrimaryKey("IdToken")]
    public class EmailConfirmationToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdToken { get; set; } // autonumérico

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(450, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string UserId { get; set; }
        
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Token { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public DateTime ExpirationDate { get; set; }
         
        public AuditTable Audit { get; set; }

    }
}
