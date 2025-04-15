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
     

        public static IServiceCollection AddAppBusinessRegistrations(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<IRegistroManager, RegistroManager>();

            return services;
        }
    }
}