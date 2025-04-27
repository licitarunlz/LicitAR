using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Core.Utils;

namespace LicitAR.Core.DI
{
    public static class DIIdentityRegistrations
    {
        public static IServiceCollection AddIdentityRegistrations(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IUserClaimsPrincipalFactory<LicitArUser>, CustomClaimsPrincipalFactory>();

            // Configuración de Identity con soporte para roles
            services.AddIdentity<LicitArUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<LicitARIdentityDbContext>()
            .AddDefaultTokenProviders();

            // Configuración de cookies
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Usuario/Login";  // Página de inicio de sesión
                options.AccessDeniedPath = "/Usuario/Register"; // Página de acceso denegado
            });

            // Configuración de autenticación externa
            services.AddAuthentication()
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = config["Authentication:Google:ClientId"] ?? "";
                    googleOptions.ClientSecret = config["Authentication:Google:ClientSecret"] ?? "";
                })
                .AddMicrosoftAccount(microsoftOptions =>
                {
                    microsoftOptions.ClientId = config["Authentication:Microsoft:ClientId"] ?? "";
                    microsoftOptions.ClientSecret = config["Authentication:Microsoft:ClientSecret"] ?? "";
                });

            // Configuración de autorización
            services.AddAuthorization();

            return services;
        }
    }
}
