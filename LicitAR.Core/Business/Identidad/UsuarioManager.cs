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
        Task<IEnumerable<UsuarioModel>> GetAllUsersAsync();
        Task<UsuarioModel> GetUserAsync(int userId);
        Task<LicitArUser> GetUserByEmailAsync(string email);
        Task<IList<string>> GetRolesAsync(LicitArUser user);
        Task<IList<Claim>> GetRoleClaimsAsync(string role);

        Task<bool> UpdateUserAsync(UsuarioModel model, int userId);
        Task<bool> ToggleUserEnabledAsync(int userId, bool enabled);
        Task<bool> ConfirmEmailAsync(string Token, string Email);
        bool IsEmailConfirmed(LicitArUser user);
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

        public async Task<UsuarioModel?> GetUserAsync(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
            if (user == null)
                return null;

            return new UsuarioModel
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email ?? "",
                FechaNacimiento = user.FechaNacimiento,
                Cuit = user.Cuit
            };

        }

        public async Task<IEnumerable<UsuarioModel>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(user => new UsuarioModel
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email ?? "",
                FechaNacimiento = user.FechaNacimiento,
                Cuit = user.Cuit,
                Enabled = user.Enabled
            });
        }
        public async Task<bool> UpdateUserAsync(UsuarioModel model, int userId)
        {
            _logger.LogInformation("Starting UpdateUserAsync for user ID: {userId}", userId);

            try
            {
                // Buscar el usuario por su ID
                _logger.LogInformation("Fetching user with ID: {userId}", userId);
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {userId} not found.", userId);
                    return false;
                }

                // Log current user data
                _logger.LogInformation("Current user data: {@User}", user);

                // Actualizar solo los campos editables
                _logger.LogInformation("Updating user properties for user ID: {userId}",userId);
                user.Nombre = model.Nombre;
                user.Apellido = model.Apellido;
                user.Email = model.Email;
                user.FechaNacimiento = model.FechaNacimiento;
                user.Cuit = model.Cuit;

                user.Audit = AuditHelper.SetModificationData(user.Audit, userId);

                // Marcar solo los campos modificados
                _logger.LogInformation("Marking modified properties for user ID: {userId}", userId);
                _dbContext.Entry(user).Property(u => u.Nombre).IsModified = true;
                _dbContext.Entry(user).Property(u => u.Apellido).IsModified = true;
                _dbContext.Entry(user).Property(u => u.Email).IsModified = true;
                _dbContext.Entry(user).Property(u => u.FechaNacimiento).IsModified = true;
                _dbContext.Entry(user).Property(u => u.Cuit).IsModified = true;

                // Guardar los cambios
                _logger.LogInformation("Saving changes for user ID: {userId}", userId);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Successfully updated user ID: {userid}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while updating user ID: {userid}", userId);
                return false;
            }
        }


        public async Task<bool> ToggleUserEnabledAsync(int userId, bool enabled)
        {
            _logger.LogInformation("Starting ToggleUserEnabledAsync for user ID: {userId} with enabled: {enabled}", userId, enabled);

            try
            {
                // Buscar el usuario por su ID
                _logger.LogInformation("Fetching user with ID: {userId}");
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {userId} not found.");
                    return false;
                }

                // Log current user state
                _logger.LogInformation("Current user state: {@User}", user);

                // Actualizar el estado de habilitación
                _logger.LogInformation("Updating Enabled property for user ID: {userId} to {enabled}", userId, enabled);
                user.Enabled = enabled;

                // Marcar la propiedad como modificada
                _dbContext.Entry(user).Property(u => u.Enabled).IsModified = true;

                // Guardar los cambios
                _logger.LogInformation("Saving changes for user ID: {userId}");
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

        public async Task<LicitArUser> GetUserByEmailAsync(string email)
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

            /* Log role claims for debugging
            foreach (var claim in roleClaims)
            {
                _logger.LogInformation($"Role claim for role {role}: {claim.Type} = {claim.Value}");
            }*/

            return roleClaims;
        }

    

        public bool IsEmailConfirmed(LicitArUser user)
        {
            return user.EmailConfirmed;


        }
        public async Task<bool> ConfirmEmailAsync(string Token,  string Email)
        {
            LicitArUser? user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == Email); // await _userManager.FindByEmailAsync(Email);

            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, Token);

           /* if (result.Succeeded) 
                await _dbContext.SaveChangesAsync();
           */
            return result.Succeeded;
        }
     }
}



