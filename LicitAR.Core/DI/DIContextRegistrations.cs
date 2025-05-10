
using LicitAR.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicitAR.Core.DI
{
 
    public static class DIContextRegistrations
    {

        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddContextRegistrations(this IServiceCollection services, IConfiguration config)
        {
            // Agregar el DbContext con la conexión
            services.AddDbContext<LicitARDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
