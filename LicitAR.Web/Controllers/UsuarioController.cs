using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using LicitAR.Core.Data.Models;

namespace LicitAR.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;

    private readonly SignInManager<LicitArUser> _signInManager;
    private readonly UserManager<LicitArUser> _userManager;

    public UsuarioController(SignInManager<LicitArUser> signInManager, UserManager<LicitArUser> userManager,
    ILogger<UsuarioController> logger)
    {
        _logger = logger;

        _signInManager = signInManager;
        _userManager = userManager;
    }



    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", "Login incorrecto");
        return View();
    }


    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new LicitArUser { UserName = email, Email = email, Apellido = "", Nombre = "", Cuit = "" };
        var result = await _userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Error en el registro");
        return View();
    }



    public IActionResult GoogleLogin()
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
        return Challenge(properties, "Google");
    }

    public IActionResult MicrosoftLogin()
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", redirectUrl);
        return Challenge(properties, "Microsoft");
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

}
