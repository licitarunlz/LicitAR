using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Data.Models; // Ensure the namespace containing 'Audit' is included
using System.Security.Claims;

namespace LicitAR.Web.Helpers
{
    public static class IdentityHelper
    {
        public static int GetUserLicitARId(ClaimsPrincipal user)
        {
            // Log all claims for debugging purposes
            var claims = user.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            //Console.WriteLine("Available Claims in GetUserLicitARId:");
            //claims.ForEach(Console.WriteLine);

            // Attempt to find the LicitARId claim
            Claim userId = user.Claims.FirstOrDefault(x => x.Type.Equals("LicitARId", StringComparison.OrdinalIgnoreCase));

            if (userId == null || string.IsNullOrEmpty(userId.Value))
            {
                Console.WriteLine("Error: LicitARId claim not found or value is null.");
                throw new InvalidOperationException("The LicitARId claim was not found for the current user.");
            }

            Console.WriteLine($"LicitARId claim found: {userId.Value}");
            return int.Parse(userId.Value);
        }

        public static string GetUserLicitARGuid(ClaimsPrincipal user)
        {
            Claim? userId = user.Claims.FirstOrDefault(x => x.Type.ToLower() == ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                throw new Exception("Usuario No encontrado");
            }
            else
                return userId.Value;
        }
    }
    public static class AuditableHelper
    {
        public static Audit SetModificationData(Audit audit, int userId)
        {
            audit ??= new Audit(); // Ensure audit is not null
            audit.ModifiedBy = userId;
            audit.ModifiedDate = DateTime.UtcNow;
            return audit;
        }
    }

    // Define the Audit class if it is missing
    public class Audit
    {
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    
}
