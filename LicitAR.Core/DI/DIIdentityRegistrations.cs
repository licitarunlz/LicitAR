using LicitAR.Core.Business.Identidad;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using LicitAR.Core.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.DI
{
    public static class DIIdentityRegistrations
    {

        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddIdentityRegistrations(this IServiceCollection services, IConfiguration config)
        {


            services.AddScoped<IUserClaimsPrincipalFactory<LicitArUser>, CustomClaimsPrincipalFactory>();
            // Agregar autenticación con Google
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;

            })
            .AddCookie(IdentityConstants.ApplicationScheme)
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

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Usuario/Login";  // Página de inicio de sesión
                options.AccessDeniedPath = "/Usuario/Register"; // (Opcional) Página de acceso denegado
            });
            services.AddIdentityCore<LicitArUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<LicitARIdentityDbContext>()
                 .AddSignInManager()
                .AddDefaultTokenProviders();


            return services;
        }
    }
}
