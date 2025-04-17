using LicitAR.Core.Business.Identidad;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.DI
{
    public static class DIBusinessRegistrations
    { 
     
        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddAppBusinessRegistrations(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<IRegistroManager, RegistroManager>();
            services.AddScoped<IUsuarioManager, UsuarioManager>();

            return services;
        }
    }
}