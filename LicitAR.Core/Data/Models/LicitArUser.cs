using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LicitAR.Core.Utils;
using Microsoft.AspNetCore.Identity;

namespace LicitAR.Core.Data.Models
{
    public class LicitArUser : IdentityUser
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(100, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Apellido { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public DateTime FechaNacimiento { get; set; }
    }

    
    
}