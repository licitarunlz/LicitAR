using LicitAR.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.DI
{
    public static class DIServicesRegistrations
    {

        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddServicesRegistrations(this IServiceCollection services, IConfiguration config)
        {

            services.AddTransient<IEmailSender, SmtpEmailSender>();
            return services;
        }
    }
}
