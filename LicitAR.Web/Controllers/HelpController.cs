using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Web.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
