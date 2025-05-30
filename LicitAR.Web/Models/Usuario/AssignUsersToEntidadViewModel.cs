using LicitAR.Core.Data.Models.Identidad;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LicitAR.Web.Models.Usuario
{
    public class AssignUsersToEntidadViewModel
    {
        [Required]
        public int IdEntidadLicitante { get; set; }
        public string NombreEntidad { get; set; }

        public List<LicitArUser> AvailableUsers { get; set; } = new();
        public List<LicitArUser> AssignedUsers { get; set; } = new();

        public List<string>? SelectedToAdd { get; set; }
        public List<string>? SelectedToRemove { get; set; }
    }
}
