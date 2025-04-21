using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Business.Identidad.Usuario;
using LicitAR.Core.Business.Identidad;
using System.Security.Claims;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;
using LicitAR.Web.Models.Usuario;
using LicitAR.Core.Data.Models.Identidad;

namespace LicitAR.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IRegistroManager _registroManager;
    private readonly SignInManager<LicitArUser> _signInManager;
    private readonly IUsuarioManager _usuarioManager;
    private readonly IEmailConfirmationManager _emailConfirmationManager;

    public UsuarioController(SignInManager<LicitArUser> signInManager, IRegistroManager registroManager, IEmailConfirmationManager emailConfirmationManager,
    ILogger<UsuarioController> logger, IUsuarioManager usuarioManager)
    {
        _logger = logger;
        _registroManager = registroManager;
        _signInManager = signInManager;
        _usuarioManager = usuarioManager;
        _emailConfirmationManager = emailConfirmationManager;
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
        TempData["MensajeCarga"] = "Procesando... Por favor espere.";
        return View();
    }


    [AllowAnonymous]
    // [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Register(RegistroModel usuario)
    {
        if (ModelState.IsValid)
        {
            LicitArUser user = await _registroManager.RegistrarAsync(usuario, 1);
            if (user != null)
            {
                return RedirectToAction("RegisterOk", "Usuario");

                /*await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");*/
            }
        }
        ModelState.AddModelError("", "Error en el registro");
        return View(usuario);
    }

    [AllowAnonymous]
    public IActionResult RegisterOk()
    {
        TempData["SuccessMessage"] = "Usuario creado exitosamente!, pronto le va a llegar un correo al email registrado";
        TempData["MensajeCarga"] = "Procesando... Por favor espere.";
        return View();
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordBasic()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPasswordBasic(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

         var user = await _usuarioManager.GetUserByEmailAsync(model.Email);
         if (user == null || !(await _usuarioManager.IsEmailConfirmedAsync(user)))
         {
             // Por seguridad, no revelamos si el usuario existe o si está confirmado
             return RedirectToAction("ForgotPasswordConfirmation");
         }

         var token = await _usuarioManager.GeneratePasswordResetTokenAsync(user);
         var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

         // Enviás el mail (con SendGrid o SMTP o lo que uses)
        /* await _emailSender.SendEmailAsync(model.Email, "Resetear contraseña",
             $"Hacé click <a href='{callbackUrl}'>acá</a> para resetear tu contraseña.");
        */
        return RedirectToAction("ForgotPasswordOk");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ForgotPasswordOk()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ConfirmarUsuario(string token, string userEmail)
    {
        TempData["Token"] = token;
        TempData["Email"] = userEmail;
        return View();
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ConfirmarUsuario(string token, string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
        if (result.Succeeded)
        {

            string usuario = IdentityHelper.GetUserLicitARGuid(User);

            bool confirmarUsuario = await _emailConfirmationManager.ConfirmarTokenAsync(token, usuario);

           // var token = await _emailConfirmationManager.GetTokenByToken(token);
            
            if (confirmarUsuario)
            {

                return RedirectToAction("Index", "Home");
            }
        }

        ModelState.AddModelError("", "Login incorrecto");
        return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
            return BadRequest("Faltan datos");

        return View(new ResetPasswordViewModel { Password = "", ConfirmPassword = "", Token = token, Email = email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

     /*   var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return RedirectToAction("ResetPasswordConfirmation");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
            return RedirectToAction("ResetPasswordConfirmation");

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);
     */
        return View(model);
    }

    public async Task<IActionResult> MiPerfil()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            int idUsuario = IdentityHelper.GetUserLicitARId(User);


            var user = await _usuarioManager.GetUserAsync(idUsuario); // Método para obtener el usuario por ID
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditarMiPerfil(UsuarioModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        int IdUsuario = IdentityHelper.GetUserLicitARId(User);
        var result = await _usuarioManager.UpdateUserAsync(model, IdUsuario); // Método para actualizar el usuario
        if (!result)
        {
            ModelState.AddModelError("", "Error al actualizar el usuario.");
            return View(model);
        }
        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
        return RedirectToAction("MiPerfil");
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


        return View(user);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Edit(UsuarioModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        int IdUsuario = IdentityHelper.GetUserLicitARId(User);
        var result = await _usuarioManager.UpdateUserAsync(model, IdUsuario); // Método para actualizar el usuario
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
