using Azure.Core;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Services;
using LicitAR.Core.Utils;
using LicitAR.Web.Business.Identidad.Usuario;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        Task<LicitArUser> ConfirmarUsuario(int idUsuario);
    }

    public class RegistroManager : IRegistroManager
    {
        private readonly LicitARIdentityDbContext _context;
        private readonly ParametrosDbContext _parametrosDbContext;
        private readonly UserManager<LicitArUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailConfirmationManager _emailConfirmationManager;

        public RegistroManager(UserManager<LicitArUser> userManager, IEmailConfirmationManager emailConfirmationManager, ParametrosDbContext parametrosDbContext, IEmailSender emailSender, LicitARIdentityDbContext context)
        {
            _context = context;
            _parametrosDbContext = parametrosDbContext;
            _userManager = userManager;
            _emailSender = emailSender;
            _emailConfirmationManager = emailConfirmationManager;
        }

        public async Task<LicitArUser> ConfirmarUsuario(int idUsuario)
        {
            var usuario = await _context.Users.FirstOrDefaultAsync(x => x.IdUsuario == idUsuario);
            if (usuario != null)
            {
                usuario.EmailConfirmed = true;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                return usuario;

            }

            return null;

        }

        public async Task<LicitArUser> RegistrarAsync(RegistroModel usuario, int IdUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            LicitArUser user = null;
            EmailConfirmationToken token = null;
            bool success = false;
            try
            {


                user = new LicitArUser { UserName = usuario.Email, Email = usuario.Email, Apellido = usuario.Apellido, Nombre = usuario.Nombre, Cuit = usuario.Cuit, FechaNacimiento = usuario.FechaNacimiento };

                user.Audit = AuditHelper.GetCreationData(IdUsuario);

                var result = await _userManager.CreateAsync(user, usuario.Password);

                token = await _emailConfirmationManager.CreateTokenAsync(user);


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                success = true;

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;

            }
            finally
            {
                if (success && token != null && user != null)
                {
                    var url = _parametrosDbContext.Parametria.FirstOrDefault(x => x.Clave.ToLower() == "urlaplicacion").Valor;

                    var callbackUrl = url + "/Usuario/ConfirmarUsuario?token=" + token.Token + "userEmail=" + user.Email;


                    //envío mail de verificación
                    await _emailSender.SendEmailAsync(user.UserName, "LicitAR - Confirmar Email", $"Hacé click <a href='{callbackUrl}'>acá</a> para Confirmar tu cuenta.");
                }
            }
            return user;


        }
    }
}
