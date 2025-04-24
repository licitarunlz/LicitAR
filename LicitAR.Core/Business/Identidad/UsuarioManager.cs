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
    }

    public class UsuarioManager : IUsuarioManager
    {
        private readonly LicitARIdentityDbContext _context;
        private readonly UserManager<LicitArUser> _userManager;
        private readonly ILogger<UsuarioManager> _logger;

        public UsuarioManager(UserManager<LicitArUser> userManager, LicitARIdentityDbContext context, ILogger<UsuarioManager> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UsuarioModel> GetUserAsync(int userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);

            return new UsuarioModel
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                FechaNacimiento = user.FechaNacimiento,
                Cuit = user.Cuit
            };

        }

        public async Task<IEnumerable<UsuarioModel>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            return users.Select(user => new UsuarioModel
            {
                IdUsuario = user.IdUsuario,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                FechaNacimiento = user.FechaNacimiento,
                Cuit = user.Cuit,
                Enabled = user.Enabled
            });
        }
        public async Task<bool> UpdateUserAsync(UsuarioModel model, int userId)
        {
            // Buscar el usuario por su ID
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUsuario == model.IdUsuario);
            if (user == null)
            {
                return false;
            }

            // Actualizar solo los campos editables
            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.Email = model.Email;
            user.FechaNacimiento = model.FechaNacimiento;
            user.Cuit = model.Cuit;

            user.Audit = AuditHelper.SetModificationData(user.Audit, userId);

            // Marcar solo los campos modificados
            _context.Entry(user).Property(u => u.Nombre).IsModified = true;
            _context.Entry(user).Property(u => u.Apellido).IsModified = true;
            _context.Entry(user).Property(u => u.Email).IsModified = true;
            _context.Entry(user).Property(u => u.FechaNacimiento).IsModified = true;
            _context.Entry(user).Property(u => u.Cuit).IsModified = true; 

            // Guardar los cambios
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserEnabledAsync(int userId, bool enabled)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
            if (user == null)
            {
                return false;
            }

            user.Enabled = enabled;

            _context.Entry(user).Property(u => u.Enabled).IsModified = true;
            await _context.SaveChangesAsync();
            return true;
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
            var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == role);
            if (roleEntity == null)
            {
                _logger.LogWarning($"Role not found: {role}");
                return new List<Claim>();
            }

            var roleClaims = await _context.RoleClaims
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

    }
}



