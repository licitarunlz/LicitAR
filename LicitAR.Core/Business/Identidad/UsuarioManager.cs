using LicitAR.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Identidad
{
    public interface IUsuarioManager
    {
        Task<IEnumerable<UsuarioModel>> GetAllUsersAsync();
    }

    public class UsuarioManager : IUsuarioManager
    {
        private readonly UserManager<LicitArUser> _userManager;

        public UsuarioManager(UserManager<LicitArUser> userManager)
        {
            _userManager = userManager;
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