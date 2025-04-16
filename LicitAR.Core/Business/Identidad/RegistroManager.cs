using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using LicitAR.Web.Business.Identidad.Usuario;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Business.Identidad
{
    public interface IRegistroManager
    {
        Task<LicitArUser> RegistrarAsync(RegistroModel usuario, int idUsuario);
    }

    public class RegistroManager : IRegistroManager
    {
        private readonly LicitARIdentityDbContext _context;
        private readonly UserManager<LicitArUser> _userManager;
        public RegistroManager(UserManager<LicitArUser> userManager, LicitARIdentityDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<LicitArUser> RegistrarAsync(RegistroModel usuario, int IdUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {


                var user = new LicitArUser { UserName = usuario.Email, Email = usuario.Email, Apellido = usuario.Apellido, Nombre = usuario.Nombre, Cuit = usuario.Cuit, FechaNacimiento = usuario.FechaNacimiento };

                user.Audit = AuditHelper.GetCreationData(IdUsuario);

                var result = await _userManager.CreateAsync(user, usuario.Password);


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return user;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;

            }
            return null;


        }
    }
}
