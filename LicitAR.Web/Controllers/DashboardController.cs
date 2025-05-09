using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Helpers.Authorization;

namespace LicitAR.Web.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/Index
        [AuthorizeClaim("Dashboard.Ver")]
        public IActionResult Index()
        {
            // Aquí puedes agregar lógica para obtener datos dinámicos para la vista
            return View();
        }
    }
}
