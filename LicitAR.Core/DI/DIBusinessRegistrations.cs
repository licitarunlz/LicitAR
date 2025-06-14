using LicitAR.Core.Business.Identidad;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Business.Dashboard;
using LicitAR.Core.Business.Parametros;
using LicitAR.Core.Utils;
using LicitAR.Core.Business.Auditoria;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LicitAR.Core.Business.Reportes;
using LicitAR.Core.Business.Documentacion;

namespace LicitAR.Core.DI
{
    public static class DIBusinessRegistrations
    { 
     
        /*Acá se ponen las registraciones de los managers de nuestro negocio*/
        public static IServiceCollection AddAppBusinessRegistrations(this IServiceCollection services, IConfiguration config)
        {

            services.AddScoped<IMessageManager, MessageManager>();
            services.AddScoped<ILicitacionManager, LicitacionManager>();

            services.AddScoped<ILicitacionDocumentacionManager, LicitacionDocumentacionManager>();
            services.AddScoped<IOfertaManager, OfertaManager>();
            services.AddScoped<IEntidadLicitanteManager, EntidadLicitanteManager>();
            services.AddScoped<IPersonaManager,PersonaManager>();
            services.AddScoped<IRegistroManager, RegistroManager>();
            services.AddScoped<IUsuarioManager, UsuarioManager>();
            services.AddScoped<IRolManager, RolManager>();
            services.AddScoped<IParametrosManager, ParametrosManager>();
            services.AddScoped<IEvaluacionManager, EvaluacionManager>();
            services.AddScoped<ILicitacionInvitacionManager, LicitacionInvitacionManager>();
            services.AddScoped<IReportesManager, ReportesManager>();
            services.AddScoped<IAuditManager, AuditManager>();
            services.AddScoped<IDashboardManager, DashboardManager>();

            return services;
        }
    }
}