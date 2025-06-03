using LicitAR.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using LicitAR.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Utils;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using LicitAR.Core.Data.Models.Identidad;

namespace LicitAR.Core.Business.Identidad
{
    public interface IUsuarioManager
    {
        Task<IEnumerable<LicitArUser>> GetAllUsersAsync();
        Task<LicitArUser?> GetUserAsync(int userId);
        Task<LicitArUser?> GetUserByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(LicitArUser user);
        Task<IList<Claim>> GetRoleClaimsAsync(string role);
        Task<IEnumerable<LicitArUser>> GetUsersInRoleAsync(string roleId);
        Task UpdateUsersInRoleAsync(string roleId, IEnumerable<string> toAdd, IEnumerable<string> toRemove);
        Task<bool> UpdateUserAsync(LicitArUser user);
        Task<bool> ToggleUserEnabledAsync(int userId, bool enabled);
        Task<bool> ConfirmEmailAsync(string token, string email);
        bool IsEmailConfirmed(LicitArUser user);
        Task<UsuarioModel?> GetUsuarioModelAsync(int userId);
        Task<IEnumerable<UsuarioModel>> GetAllUsuarioModelsAsync();
        Task<Dictionary<int, (string Email, string Nombre, string Apellido)>> GetUsuariosInfoByIdsAsync(IEnumerable<int> idsUsuario);
    }

    public class UsuarioManager : IUsuarioManager
    {
        private readonly LicitARDbContext _dbContext;
        private readonly UserManager<LicitArUser> _userManager;
        private readonly ILogger<UsuarioManager> _logger;

        public UsuarioManager(UserManager<LicitArUser> userManager, LicitARDbContext dbContext, ILogger<UsuarioManager> logger)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<LicitArUser?> GetUserAsync(int userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
        }

        public async Task<IEnumerable<LicitArUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IEnumerable<LicitArUser>> GetUsersInRoleAsync(string roleId)
        {
            var userIds = await _dbContext.UserRoles
                .Where(ur => ur.RoleId == roleId)
                .Select(ur => ur.UserId)
                .ToListAsync();

            return await _dbContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToListAsync();
        }

        public async Task UpdateUsersInRoleAsync(string roleId, IEnumerable<string> toAdd, IEnumerable<string> toRemove)
        {
            foreach (var userId in toAdd)
            {
                if (!await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId))
                {
                    _dbContext.UserRoles.Add(new IdentityUserRole<string>
                    {
                        UserId = userId,
                        RoleId = roleId
                    });
                }
            }

            foreach (var userId in toRemove)
            {
                var userRole = await _dbContext.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole != null)
                {
                    _dbContext.UserRoles.Remove(userRole);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserAsync(LicitArUser user)
        {
            _logger.LogInformation("Starting UpdateUserAsync for user ID: {userId}", user.IdUsuario);

            try
            {
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario);
                if (existingUser == null)
                {
                    _logger.LogWarning("User with ID {userId} not found.", user.IdUsuario);
                    return false;
                }

                // Update fields
                existingUser.Nombre = user.Nombre;
                existingUser.Apellido = user.Apellido;
                existingUser.Email = user.Email;
                existingUser.FechaNacimiento = user.FechaNacimiento;
                existingUser.Cuit = user.Cuit;
                existingUser.Enabled = user.Enabled;

                existingUser.Audit = AuditHelper.SetModificationData(existingUser.Audit, user.IdUsuario);

                _dbContext.Entry(existingUser).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated user ID: {userId}", user.IdUsuario);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating user ID: {userId}", user.IdUsuario);
                return false;
            }
        }

        public async Task<bool> ToggleUserEnabledAsync(int userId, bool enabled)
        {
            _logger.LogInformation("Starting ToggleUserEnabledAsync for user ID: {userId} with enabled: {enabled}", userId, enabled);

            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {userId} not found.");
                    return false;
                }

                user.Enabled = enabled;

                _dbContext.Entry(user).Property(u => u.Enabled).IsModified = true;

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated Enabled property for user ID: {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while toggling Enabled property for user ID: {userId}");
                return false;
            }
        }

        public async Task<LicitArUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IList<string>> GetRolesAsync(LicitArUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IList<Claim>> GetRoleClaimsAsync(string role)
        {
            var roleEntity = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (roleEntity == null)
            {
                _logger.LogWarning($"Role not found: {role}");
                return new List<Claim>();
            }

            var roleClaims = await _dbContext.RoleClaims
                .Where(rc => rc.RoleId == roleEntity.Id)
                .Select(rc => new Claim(rc.ClaimType, rc.ClaimValue))
                .ToListAsync();

            return roleClaims;
        }

        public bool IsEmailConfirmed(LicitArUser user)
        {
            return user.EmailConfirmed;
        }

        public async Task<bool> ConfirmEmailAsync(string token, string email)
        {
            LicitArUser? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task<UsuarioModel?> GetUsuarioModelAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            return user == null ? null : MapToUsuarioModel(user);
        }

        public async Task<IEnumerable<UsuarioModel>> GetAllUsuarioModelsAsync()
        {
            var users = await GetAllUsersAsync();
            return users.Select(MapToUsuarioModel);
        }

        /// <summary>
        /// Devuelve un diccionario idUsuario -> (Email, Nombre, Apellido)
        /// </summary>
        public async Task<Dictionary<int, (string Email, string Nombre, string Apellido)>> GetUsuariosInfoByIdsAsync(IEnumerable<int> idsUsuario)
        {
            return await _dbContext.Users
                .Where(u => idsUsuario.Contains(u.IdUsuario))
                .Select(u => new { u.IdUsuario, u.Email, u.Nombre, u.Apellido })
                .ToDictionaryAsync(
                    u => u.IdUsuario,
                    u => (u.Email ?? "", u.Nombre ?? "", u.Apellido ?? "")
                );
        }

        private UsuarioModel MapToUsuarioModel(LicitArUser user)
        {
            return new UsuarioModel
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                FechaNacimiento = user.FechaNacimiento,
                Cuit = user.Cuit,
                Enabled = user.Enabled,
                EmailConfirmed = user.EmailConfirmed
            };
        }
    }
}



