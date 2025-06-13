using RazorLight;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace LicitAR.Web.Services
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model, object? viewData = null);
    }

    public class ViewRenderService : IViewRenderService
    {
        private readonly RazorLightEngine _razorEngine;

        public ViewRenderService()
        {
            var assembly = typeof(ViewRenderService).Assembly;
            var rootNamespace = "LicitAR.Web"; // Ajusta si tu namespace raíz es diferente
            _razorEngine = new RazorLightEngineBuilder()
                .UseEmbeddedResourcesProject(assembly, rootNamespace)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderToStringAsync(string viewName, object model, object? viewData = null)
        {
            // viewName should be the filename, e.g. "LicitacionCreada.cshtml"
            return await _razorEngine.CompileRenderAsync(viewName + ".cshtml", model);
        }
    }
}
