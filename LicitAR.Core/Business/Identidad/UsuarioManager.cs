using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Identidad
{
    public interface IUsuarioManager
    {
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


    }
}
