using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
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
        private readonly LicitARIdentityDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<LicitArUser> _userManager;

        public CustomClaimsPrincipalFactory(
            LicitARIdentityDbContext context,
            UserManager<LicitArUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(LicitArUser user)
        {
            Console.WriteLine("[Generating Claims...] ");
            var identity = await base.GenerateClaimsAsync(user);
            //Console.WriteLine($"User:: {user.Email} - {user.Id}");
            var userFromDB = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
            if (userFromDB != null)
            {
                // Add LicitARId claim
                identity.AddClaim(new Claim("LicitARId", userFromDB.IdUsuario.ToString()));
                Console.WriteLine($"LicitARId claim added: {userFromDB.IdUsuario}");
            }
            else
            {
                Console.WriteLine("Error: User not found in database for ID: " + user.Id);
            }

            // Add ClaimTypes.Name and ClaimTypes.Email
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

            // Add roles and associated permission claims
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleName));

                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var rc in roleClaims)
                    {
                        // Avoid duplicate permission claims
                        if (rc.Type == "permission" && !identity.Claims.Any(c => c.Type == "permission" && c.Value == rc.Value))
                        {
                            identity.AddClaim(rc);
                        }
                    }
                }
            }

            return identity;
        }
    }
}
