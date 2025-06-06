using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Web.Controllers
{
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

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

        public IActionResult ConnectionError()
        {
            return View();
        }

        public IActionResult Generic()
        {
            var errorId = TempData["ErrorId"] ?? Guid.NewGuid().ToString();
            _logger.LogError("An error occurred while processing your request. Error ID: {ErrorId}", errorId);
            ViewBag.ErrorMessage = TempData["ErrorMessage"] ?? "Ocurrió un error inesperado.";
            return View();
        }
    }
}
