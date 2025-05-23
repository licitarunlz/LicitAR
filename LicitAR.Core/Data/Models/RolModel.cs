using Microsoft.AspNetCore.Identity;

namespace LicitAR.Core.Data.Models
{
    public class RolModel
    {
        public IdentityRole Rol { get; set; }
        public int CantidadUsuarios { get; set; }
    }
}
