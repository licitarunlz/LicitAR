using RazorLight;
using System.IO;
using System.Threading.Tasks;

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
            var project = new EmbeddedRazorProject(assembly, rootNamespace + ".Views.EmailTemplates");
            _razorEngine = new RazorLightEngineBuilder()
                .UseProject(project)
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
