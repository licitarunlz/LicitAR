using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers.Authorization;
using LicitAR.Web.Services;

namespace LicitAR.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly EmailSenderService _emailSenderService;
    private readonly IConfiguration _config;

    public HomeController(ILogger<HomeController> logger, EmailSenderService emailSenderService, IConfiguration config)
    {
        _logger = logger;
        _emailSenderService = emailSenderService;
        _config = config;
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

    [HttpGet]
    [AllowAnonymous]
    [Route("Home/TestEmail")]
    public async Task<IActionResult> TestEmail()
    {
        var baseUrl = _config["App:BaseUrl"] ?? "https://tusitio.com";
        var licitacionId = 123;
        var model = new
        {
            LicitacionNombre = "Licitación de Prueba",
            LicitacionId = licitacionId,
            FechaCreacion = DateTime.Now,
            UrlDetalle = $"{baseUrl}/licitacion/Details/{licitacionId}"
        };

        // Cambia el destinatario por el tuyo para pruebas
        await _emailSenderService.SendLicitacionCreadaAsync("pablo.numb@gmail.com", model);

        // Retorna la vista de resultado
        return View("TestEmailResult", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
