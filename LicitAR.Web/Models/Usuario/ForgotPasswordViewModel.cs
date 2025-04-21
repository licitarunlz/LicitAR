using LicitAR.Core.Utils;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Web.Models.Usuario
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = ErrorMessages.VALIDEMAIL)]
        public required string Email { get; set; }
    }
}
