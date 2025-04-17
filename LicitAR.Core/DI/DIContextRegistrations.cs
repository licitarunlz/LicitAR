using LicitAR.Core.Business.Identidad;
using LicitAR.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.DI
{
 
    public static class DIContextRegistrations
    {

        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddContextRegistrations(this IServiceCollection services, IConfiguration config)
        {
            // Agregar el DbContext con la conexión
            services.AddDbContext<LicitARIdentityDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            //Agregar el DbContext con la conexión de los Parámetros
            services.AddDbContext<ParametrosDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddDbContext<ActoresDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddDbContext<LicitacionesDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
