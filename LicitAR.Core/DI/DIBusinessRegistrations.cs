using LicitAR.Core.Business.Identidad;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Business.Parametros;
using LicitAR.Core.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LicitAR.Core.DI
{
    public static class DIBusinessRegistrations
    { 
     
        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddAppBusinessRegistrations(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<IMessageManager, MessageManager>();
            services.AddScoped<ILicitacionManager, LicitacionManager>();
            services.AddScoped<IOfertaManager, OfertaManager>();
            services.AddScoped<IEntidadLicitanteManager, EntidadLicitanteManager>();
            services.AddScoped<IPersonaManager,PersonaManager>();
            services.AddScoped<IRegistroManager, RegistroManager>();
            services.AddScoped<IUsuarioManager, UsuarioManager>();
            services.AddScoped<IRolManager, RolManager>();
            services.AddScoped<IParametrosManager, ParametrosManager>();
            services.AddScoped<IEvaluacionManager, EvaluacionManager>();

            return services;
        }
    }
}