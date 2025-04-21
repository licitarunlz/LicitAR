using LicitAR.Core.Data.Models.Helpers;
using System.Security.Claims;

namespace LicitAR.Web.Helpers
{
    public static class IdentityHelper
    {
        public static int GetUserLicitARId(ClaimsPrincipal user)
        {
            Claim userId = user.Claims.FirstOrDefault(x => x.Type.ToLower() == "licitarid");

            if (userId == null)
            {
                throw new Exception("Usuario No encontrado");
            }
            else
                return int.Parse(userId.Value);
        }

        public static string GetUserLicitARGuid(ClaimsPrincipal user)
        {
            Claim userId = user.Claims.FirstOrDefault(x => x.Type.ToLower() == ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                throw new Exception("Usuario No encontrado");
            }
            else
                return userId.Value;
        }

    }
}
