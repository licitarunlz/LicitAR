using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers.Authorization;

namespace LicitAR.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [AuthorizeClaim("Inicio.Ver")]
    public IActionResult Index()
    {
        return View();
    }

    [AuthorizeClaim("Inicio.Ver")]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
