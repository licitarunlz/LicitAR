using LicitAR.Core.Data.Models.Identidad;

namespace LicitAR.Web.Models.Usuario
{
    public class AssignUsersToRoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<LicitArUser> AvailableUsers { get; set; } = new();
        public List<LicitArUser> AssignedUsers { get; set; } = new();
        public List<string> SelectedToAdd { get; set; } = new();
        public List<string> SelectedToRemove { get; set; } = new();
    }
}
