using Microsoft.AspNetCore.Identity;

namespace LicitAR.Core.Business.Identidad
{
    public interface IRolManager
    {
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityRole?> GetRoleByIdAsync(string roleId);
    }

    public class RolManager : IRolManager
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolManager(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
        {
            return await Task.FromResult(_roleManager.Roles);
        }

        public async Task<IdentityRole?> GetRoleByIdAsync(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }
    }
}
