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
        private readonly LicitARDbContext _dbContext;
        private readonly UserManager<LicitArUser> _userManager;
        private readonly IEmailSender _emailSender;

        public RegistroManager(LicitARDbContext dbContext, UserManager<LicitArUser> userManager, IEmailSender emailSender)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<LicitArUser> RegistrarAsync(RegistroModel usuario, int IdUsuario)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            LicitArUser? user = null;
            string? token = null;
            bool success = false;
            try
            {
                var existenciaUsuario = await _dbContext.Users.FirstOrDefaultAsync(x => x.Cuit == usuario.Cuit || x.UserName == usuario.Email);

                if (existenciaUsuario != null)
                {
                    throw new Exception("Usuario ya existente dentro de LicitAr");
                }

                user = new LicitArUser { UserName = usuario.Email, Email = usuario.Email, Apellido = usuario.Apellido, Nombre = usuario.Nombre, Cuit = usuario.Cuit, FechaNacimiento = usuario.FechaNacimiento };

                user.Audit = AuditHelper.GetCreationData(IdUsuario);

                var result = await _userManager.CreateAsync(user, usuario.Password);

                await _userManager.AddToRoleAsync(user, "Oferente");

                token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _dbContext.SaveChangesAsync();

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
                    var url = _dbContext.Parametria.FirstOrDefault(x => x.Clave.ToLower() == "urlaplicacion")?.Valor;

                    string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var callbackUrl = url + "/Usuario/ConfirmarUsuario?token=" + encodedToken + "&userEmail=" + user.Email;

                    // Mensaje HTML mejorado para el mail de confirmación de cuenta
                    var htmlMessage = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; border: 1px solid #eee; border-radius: 8px; padding: 24px; background: #fafbfc;'>
                            <h2 style='color: #2d3e50;'>Confirmá tu cuenta</h2>
                            <p>Hola <b>{user.Nombre} {user.Apellido}</b>,</p>
                            <p>¡Gracias por registrarte en <b>LicitAR</b>!</p>
                            <p>Para activar tu cuenta, por favor hacé click en el siguiente botón:</p>
                            <p style='text-align: center; margin: 32px 0;'>
                                <a href='{callbackUrl}' style='background: #28a745; color: #fff; text-decoration: none; padding: 12px 24px; border-radius: 4px; font-weight: bold; display: inline-block;'>Confirmar cuenta</a>
                            </p>
                            <p>Si no creaste esta cuenta, podés ignorar este correo.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 24px 0;'/>
                            <p style='font-size: 12px; color: #888;'>Este enlace expirará después de su primer uso.</p>
                        </div>
                    ";

                    await _emailSender.SendEmailAsync(user.UserName ?? "", "LicitAR - Confirmar Email", htmlMessage);
                }
            }
            return user;
        }

        public async Task<LicitArUser?> BlanquearPasswordAsync(string email)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            LicitArUser? user = null;
            string? token = null;
            bool success = false;
            try
            {
                user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

                if (user != null)
                {
                    token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    await _dbContext.SaveChangesAsync();

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
                    var url = _dbContext.Parametria.FirstOrDefault(x => x.Clave.ToLower() == "urlaplicacion")?.Valor;
                    string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    url += "/Usuario/ResetPassword?token=" + encodedToken + "&email=" + user.Email;

                    // Mensaje HTML mejorado para el mail de reseteo de contraseña
                    var htmlMessage = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto; border: 1px solid #eee; border-radius: 8px; padding: 24px; background: #fafbfc;'>
                            <h2 style='color: #2d3e50;'>Restablecer tu contraseña</h2>
                            <p>Hola <b>{user.Nombre} {user.Apellido}</b>,</p>
                            <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta en <b>LicitAR</b>.</p>
                            <p>Para continuar, hacé click en el siguiente botón:</p>
                            <p style='text-align: center; margin: 32px 0;'>
                                <a href='{url}' style='background: #007bff; color: #fff; text-decoration: none; padding: 12px 24px; border-radius: 4px; font-weight: bold; display: inline-block;'>Restablecer contraseña</a>
                            </p>
                            <p>Si no solicitaste este cambio, podés ignorar este correo.</p>
                            <hr style='border: none; border-top: 1px solid #eee; margin: 24px 0;'/>
                            <p style='font-size: 12px; color: #888;'>Este enlace expirará después de su primer uso.</p>
                        </div>
                    ";

                    await _emailSender.SendEmailAsync(user.Email ?? "", "Resetear contraseña", htmlMessage);
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
                user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
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
