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
        private readonly RazorLightEngine _engine;

        public ViewRenderService()
        {
            var templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "EmailTemplates");
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(templatesPath)
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> RenderToStringAsync(string viewName, object model, object? viewData = null)
        {
            // viewName should be the filename, e.g. "LicitacionCreada.cshtml"
            return await _engine.CompileRenderAsync(viewName + ".cshtml", model);
        }
    }
}
