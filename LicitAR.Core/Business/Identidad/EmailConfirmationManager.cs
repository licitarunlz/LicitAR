using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;
using LicitAR.Core.Utils;

namespace LicitAR.Core.Business.Identidad
{
    public interface IEmailConfirmationManager
    {
        Task<EmailConfirmationToken> CreateTokenAsync(LicitArUser licitArUser);
        Task<EmailConfirmationToken> GetTokenByTokenAsync(string token);
        Task<bool> ConfirmarTokenAsync(string Token, string usuario);
    }

    public class EmailConfirmationManager : IEmailConfirmationManager
    {
        private LicitARIdentityDbContext _context;

        public EmailConfirmationManager(LicitARIdentityDbContext context)
        {
            _context = context;
        }

        public async Task<EmailConfirmationToken> GetTokenByTokenAsync(string token)
        {
            var result = await _context.AspNetUserEmailConfirmationTokens.FirstOrDefaultAsync<EmailConfirmationToken>(x => x.Token.ToUpper() == token.ToUpper());

            return result;
        }

        public async Task<EmailConfirmationToken> CreateTokenAsync(LicitArUser licitArUser)
        {
            var audit = AuditHelper.GetCreationData(licitArUser.IdUsuario);

            EmailConfirmationToken emailConfirmationToken = new EmailConfirmationToken
            {
                UserId = licitArUser.Id,
                Token = Guid.NewGuid().ToString(),
                Audit = audit,
                ExpirationDate = DateTime.UtcNow.AddDays(3)

            };

            var result = await _context.AspNetUserEmailConfirmationTokens.AddAsync(emailConfirmationToken);

            await _context.SaveChangesAsync();

            return result.Entity;

        }
    
    
        public async Task<bool> ConfirmarTokenAsync(string Token, string usuario)
        {

            try
            {
                var token = await this.GetTokenByTokenAsync(Token);
                if (token != null)
                {

                    if (token.UserId == usuario)
                    {
                        //es el mismo usuario
                        if (token.ExpirationDate > DateTime.Now)
                        {
                            //No está expirado 
                            return true;
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                throw;
            }
            return false;
        }
    }
}
