using Azure.Core;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Services;
using LicitAR.Core.Utils;
using LicitAR.Web.Business.Identidad.Usuario;
using Microsoft.AspNetCore.Components.Forms.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
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

        Task<LicitArUser?> BlanquearPasswordAsync(string email);
        Task<LicitArUser?> ResetPasswordAsync(string token, string email, string password);
    }

    public class RegistroManager : IRegistroManager
    {
        private readonly LicitARIdentityDbContext _context;
        private readonly ParametrosDbContext _parametrosDbContext;
        private readonly UserManager<LicitArUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RegistroManager(UserManager<LicitArUser> userManager, ParametrosDbContext parametrosDbContext, IEmailSender emailSender, LicitARIdentityDbContext context)
        {
            _context = context;
            _parametrosDbContext = parametrosDbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }



        public async Task<LicitArUser> RegistrarAsync(RegistroModel usuario, int IdUsuario)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            LicitArUser? user = null;
            string? token = null;
            bool success = false;
            try
            {
                var existenciaUsuario = await _context.Users.FirstOrDefaultAsync(x => x.Cuit == usuario.Cuit || x.UserName == usuario.Email);

                if (existenciaUsuario != null)
                {
                    throw new Exception("Usuario ya existente dentro de LicitAr");
                }

                user = new LicitArUser { UserName = usuario.Email, Email = usuario.Email, Apellido = usuario.Apellido, Nombre = usuario.Nombre, Cuit = usuario.Cuit, FechaNacimiento = usuario.FechaNacimiento };

                user.Audit = AuditHelper.GetCreationData(IdUsuario);

                var result = await _userManager.CreateAsync(user, usuario.Password);


                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                success = true;

            }
            catch
            {
                await transaction.RollbackAsync();
                throw;

            }
            finally
            {
                if (success && token != null && user != null)
                {
                    var url = _parametrosDbContext.Parametria.FirstOrDefault(x => x.Clave.ToLower() == "urlaplicacion")?.Valor;

                    string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var callbackUrl = url + "/Usuario/ConfirmarUsuario?token=" + encodedToken + "&userEmail=" + user.Email;


                    //envío mail de verificación
                    await _emailSender.SendEmailAsync(user.UserName ?? "", "LicitAR - Confirmar Email", $"Hacé click <a href='{callbackUrl}'>acá</a> para Confirmar tu cuenta.");
                }
            }
            return user;


        }

        public async Task<LicitArUser?> BlanquearPasswordAsync(string email)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            LicitArUser? user = null;
            string? token = null;
            bool success = false;
            try
            {
                user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user != null)
                {

                    token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                    success = true;
                }
            }
            catch
            {
                user = null;
                await transaction.RollbackAsync();
                throw;

            }
            finally
            {
                if (success && token != null && user != null)
                {

                    var url = _parametrosDbContext.Parametria.FirstOrDefault(x => x.Clave.ToLower() == "urlaplicacion")?.Valor;
                    string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    url += "/Usuario/ResetPassword?token=" + encodedToken + "&email=" + user.Email;
                    // Enviás el mail (con SendGrid o SMTP o lo que uses)
                    await _emailSender.SendEmailAsync(user.Email ?? "", "Resetear contraseña",
                        $"Hacé click <a href='{url}'>acá</a> para resetear tu contraseña.");
                }
            }
            return user;
        }

        public async Task<LicitArUser?> ResetPasswordAsync(string encodedtoken, string email, string password)
        {

            LicitArUser? user = null;
            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedtoken));



            try
            {
                user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user != null)
                {
                    var identityResult = await _userManager.ResetPasswordAsync(user, token, password);

                    if (!identityResult.Succeeded)
                    {
                        throw new Exception(identityResult.Errors.ToString());
                    }
                }


            }
            catch
            {
                throw;

            }
            return user;
        }
    }
}
