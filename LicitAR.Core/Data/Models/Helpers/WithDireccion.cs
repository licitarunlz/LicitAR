using LicitAR.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Data.Models.Helpers
{
    public class WithDireccion
    {
        [DisplayName("Provincia")]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdProvincia { get; set; }

        [DisplayName("Localidad")]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdLocalidad { get; set; }

        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Barrio")]
        public string? DireccionBarrio { get; set; }
        [MaxLength(200, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Calle")]
        public string? DireccionCalle { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Nro.")]
        public string? DireccionNumero { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Piso")]
        public string? DireccionPiso { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("Depto")]
        public string? DireccionDepto { get; set; }
        [MaxLength(10, ErrorMessage = ErrorMessages.MAXLENGTH)]

        [DisplayName("C.P.A.")]
        public string? DireccionCodigoPostal { get; set; }

    }

}
