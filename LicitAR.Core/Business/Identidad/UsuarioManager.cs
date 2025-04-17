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

namespace LicitAR.Core.Business.Identidad
{
    public interface IUsuarioManager
    {
        Task<IEnumerable<UsuarioModel>> GetAllUsersAsync();
        Task<UsuarioModel> GetUserAsync(int userId);
        Task<bool> UpdateUserAsync(UsuarioModel model, int userId);
        Task<bool> ToggleUserEnabledAsync(int userId, bool enabled);
    }

    public class UsuarioManager : IUsuarioManager
    {
        private readonly LicitARIdentityDbContext _context;
        private readonly UserManager<LicitArUser> _userManager;

        public UsuarioManager(UserManager<LicitArUser> userManager, LicitARIdentityDbContext context)
        {
            _context = context;
            _userManager = userManager;
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
                Cuit = user.Cuit
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
    }
}



