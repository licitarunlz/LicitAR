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

namespace LicitAR.Core.Business.Identidad
{
    public interface IUsuarioManager
    {
        Task<IEnumerable<UsuarioModel>> GetAllUsersAsync();
        LicitArUser GetUser(int UserId);
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
    }
}



