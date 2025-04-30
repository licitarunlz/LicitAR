using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Web.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            // Aquí puedes agregar lógica para obtener datos dinámicos para la vista
            return View();
        }
    }
}
