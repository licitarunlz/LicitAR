using LicitAR.Core.Data;
using LicitAR.Core.Data.Models.Identidad;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Utils
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<LicitArUser>
    {
        private LicitARIdentityDbContext _context;

        public CustomClaimsPrincipalFactory(
            LicitARIdentityDbContext context,
            UserManager<LicitArUser> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            _context = context;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(LicitArUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var userFromDdbb = _context.Users.FirstOrDefault(x => x.Id == user.Id);
            identity.AddClaim(new Claim("LicitARId", userFromDdbb.IdUsuario.ToString())); // acá va tu lógica




            return identity;
        }
    }
}
