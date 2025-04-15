using LicitAR.Core.Utils; 
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Web.Business.Identidad.Usuario
{
    public class UsuarioModel
    {
        [Key]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        public int IdUsuario { get; set; }


        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Nombre { get; set; }


        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(30, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Apellido { get; set; }


        [Required(ErrorMessage = ErrorMessages.REQUIRED)] 
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }


        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(11, ErrorMessage = ErrorMessages.MAXLENGTH)] 
        public required string Cuit { get; set; }

        [EmailAddress(ErrorMessage = ErrorMessages.VALIDEMAIL)]
        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Email { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        public required string Password { get; set; }

        [Required(ErrorMessage = ErrorMessages.REQUIRED)]
        [MaxLength(50, ErrorMessage = ErrorMessages.MAXLENGTH)]
        [Display(Name = "Confirmar Password")]
        public required string ConfirmaPassword { get; set; }

    }
}
