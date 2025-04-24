using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace LicitAR.Web.Helpers.Authorization;

public class AuthorizeClaimAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _claimValue;

    public AuthorizeClaimAttribute(string claimValue)
    {
        _claimValue = claimValue; // The value to match (e.g., "Usuarios.Editar")
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new RedirectToActionResult("Login", "Usuario", null);
            return;
        }

        //Log all claims for debugging
        var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthorizeClaimAttribute>>();
        /*foreach (var claim in user.Claims)
        {
            logger?.LogInformation($"User claim: {claim.Type} = {claim.Value}");
        }*/

        //logger?.LogInformation($"Checking for claim: permission = {_claimValue}");

        // Check for "permission" as the claim type and match the value
        var hasClaim = user.Claims.Any(c => 
            string.Equals(c.Type, "permission", StringComparison.OrdinalIgnoreCase) && 
            string.Equals(c.Value, _claimValue, StringComparison.OrdinalIgnoreCase));

        if (!hasClaim)
        {
            logger?.LogWarning($"Claim not found: permission = {_claimValue}");
            context.Result = new RedirectToActionResult("NoAutorizado", "Usuario", null);
        }
    }
}