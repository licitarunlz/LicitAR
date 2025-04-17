using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Business.Identidad.Usuario;
using LicitAR.Core.Business.Identidad;

namespace LicitAR.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IRegistroManager _registroManager;
    private readonly SignInManager<LicitArUser> _signInManager;
    private readonly IUsuarioManager _usuarioManager;

    public UsuarioController(SignInManager<LicitArUser> signInManager, IRegistroManager registroManager,
    ILogger<UsuarioController> logger, IUsuarioManager usuarioManager)
    {
        _logger = logger;
        _registroManager = registroManager;
        _signInManager = signInManager;
        _usuarioManager = usuarioManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var users = await _usuarioManager.GetAllUsersAsync(); // Asegúrate de que este método devuelva datos válidos
        return View(users);
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
    public async Task<IActionResult> Register(RegistroModel usuario)
    {
        if (ModelState.IsValid) 
        {
            LicitArUser user = await _registroManager.RegistrarAsync(usuario, 1);
            if (user != null)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
        }
        ModelState.AddModelError("", "Error en el registro");
        return View();
    }



    public IActionResult MisDatos()
    {
        
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

    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _usuarioManager.GetUserAsync(id); // Método para obtener el usuario por ID
        if (user == null)
        {
            return NotFound();
        }

        // Mapear el usuario a UsuarioModel para pasarlo a la vista
        var usuarioModel = new UsuarioModel
        {
            IdUsuario = user.IdUsuario,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Email = user.Email,
            FechaNacimiento = user.FechaNacimiento,
            Cuit = user.Cuit
        };

        return View(usuarioModel);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(UsuarioModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _usuarioManager.UpdateUserAsync(model); // Método para actualizar el usuario
        if (!result)
        {
            ModelState.AddModelError("", "Error al actualizar el usuario.");
            return View(model);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ToggleEnabled(int id, bool enabled)
    {
        var result = await _usuarioManager.ToggleUserEnabledAsync(id, enabled);
        if (!result)
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }
}
