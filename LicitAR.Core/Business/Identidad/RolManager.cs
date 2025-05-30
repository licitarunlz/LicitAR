using Microsoft.AspNetCore.Identity;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data.Models.Identidad;

namespace LicitAR.Core.Business.Identidad
{
    public interface IRolManager
    {
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityRole?> GetRoleByIdAsync(string roleId);
        Task<IEnumerable<RolModel>> GetAllRolesWithResumenAsync();
    }

    public class RolManager : IRolManager
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<LicitArUser> _userManager;

        public RolManager(RoleManager<IdentityRole> roleManager, UserManager<LicitArUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles);
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<IEnumerable<RolModel>> GetAllRolesWithResumenAsync()
        {
            var roles = _roleManager.Roles.ToList();
            var result = new List<RolModel>();

            foreach (var rol in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(rol.Name);
                result.Add(new RolModel
                {
                    Rol = rol,
                    CantidadUsuarios = usersInRole.Count
                });
            }

            return result;
        }
    }
}
