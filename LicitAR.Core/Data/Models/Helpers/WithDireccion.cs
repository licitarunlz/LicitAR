using LicitAR.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models.Helpers
{
    public class WithDireccion
    {
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdProvincia { get; set; }
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLocalidad { get; set; }

        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? DireccionBarrio { get; set; }
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? DireccionCalle { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? DireccionNumero { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? DireccionPiso { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? DireccionDepto { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public string? DireccionCodigoPostal { get; set; }

    }

}
