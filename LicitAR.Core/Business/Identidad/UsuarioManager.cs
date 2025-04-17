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

namespace LicitAR.Core.Business.Identidad
{
    public interface IUsuarioManager
    {
        Task<IEnumerable<UsuarioModel>> GetAllUsersAsync();
        LicitArUser GetUser(int UserId);
        Task<LicitArUser> GetUserAsync(int userId);
        Task<bool> UpdateUserAsync(UsuarioModel model);
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
        public LicitArUser GetUser(int UserId)
        {
            var user = _context.Users.First(x => x.IdUsuario == UserId);

            return user;
        }
        public async Task<LicitArUser> GetUserAsync(int userId)
        {
            return await _userManager.Users.FirstOrDefaultAsync(u => u.IdUsuario == userId);
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
        public async Task<bool> UpdateUserAsync(UsuarioModel model)
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
    }
}



