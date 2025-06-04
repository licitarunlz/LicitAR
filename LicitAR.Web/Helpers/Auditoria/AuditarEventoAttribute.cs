using Microsoft.AspNetCore.Mvc.Filters;
using LicitAR.Core.Business.Auditoria;
using LicitAR.Web.Helpers.Auditoria;

namespace LicitAR.Web.Helpers.Auditoria
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuditarEventoAttribute : ActionFilterAttribute
    {
        private readonly string _accion;
        private readonly string _entidad;
        private readonly string _descripcion;
        private readonly string _entidadIdParam;

        /// <param name="accion">Descripción de la acción (ej: "UsuarioController - Edit")</param>
        /// <param name="entidad">Nombre de la entidad (ej: "Usuario")</param>
        /// <param name="descripcion">Breve descripción de la acción</param>
        /// <param name="entidadIdParam">Nombre del parámetro de ruta/query que representa el Id de la entidad (opcional)</param>
        public AuditarEventoAttribute(string accion, string entidad, string descripcion = "", string entidadIdParam = null)
        {
            _accion = accion;
            _entidad = entidad;
            _descripcion = descripcion;
            _entidadIdParam = entidadIdParam;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Ejecutar la acción primero
            var resultContext = await next();

            try
            {
                var httpContext = context.HttpContext;
                var services = httpContext.RequestServices;
                var auditManager = services.GetService<IAuditManager>();

                // Obtener UsuarioId desde los claims
                int usuarioId = IdentityHelper.GetUserLicitARId(httpContext.User);

                // Obtener EntidadId si corresponde
                int? entidadId = null;
                if (!string.IsNullOrEmpty(_entidadIdParam) && context.ActionArguments.ContainsKey(_entidadIdParam))
                {
                    var val = context.ActionArguments[_entidadIdParam];
                    if (val is int intVal)
                        entidadId = intVal;
                    else if (val != null && int.TryParse(val.ToString(), out int parsed))
                        entidadId = parsed;
                }

                // Obtener IP y UserAgent
                string ip = httpContext.Connection.RemoteIpAddress?.ToString();
                string userAgent = httpContext.Request.Headers["User-Agent"].ToString();

                // Registrar evento
                if (auditManager != null && usuarioId > 0)
                {
                    await auditManager.LogSystemEvent(
                        usuarioId,
                        _accion,
                        _entidad,
                        entidadId,
                        _descripcion,
                        ip,
                        userAgent
                    );
                }
            }
            catch
            {
                // No lanzar excepción si falla la auditoría
            }
        }
    }
}
