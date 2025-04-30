using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Web.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult NotFound()
        {
            return View();
        }

        public IActionResult BadRequest()
        {
            return View();
        }

        public IActionResult Unauthorized()
        {
            return View();
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        public IActionResult InternalServerError()
        {
            return View();
        }
    }
}
