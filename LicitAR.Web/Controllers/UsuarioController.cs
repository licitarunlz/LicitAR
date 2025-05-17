using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Business.Identidad;
using LicitAR.Core.Data.Models.Identidad;
using LicitAR.Web.Business.Identidad.Usuario;
using LicitAR.Web.Helpers;
using LicitAR.Web.Helpers.Authorization;
using LicitAR.Web.Models.Usuario;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using LicitAR.Core.Business.Licitaciones;
using System.Security.Claims;

namespace LicitAR.Web.Controllers;

public class UsuarioController : Controller
{
    private readonly ILogger<UsuarioController> _logger;
    private readonly IRegistroManager _registroManager;
    private readonly SignInManager<LicitArUser> _signInManager;
    private readonly IUsuarioManager _usuarioManager;
    private readonly IRolManager _rolManager;
    private readonly IPersonaManager _personaManager;

    public UsuarioController(SignInManager<LicitArUser> signInManager, IRegistroManager registroManager,
    ILogger<UsuarioController> logger, IUsuarioManager usuarioManager, IRolManager rolManager, IPersonaManager personaManager)
    {
        _logger = logger;
        _registroManager = registroManager;
        _signInManager = signInManager;
        _usuarioManager = usuarioManager;
        _rolManager = rolManager;
        _personaManager = personaManager;
    }

    [Authorize]
    [AuthorizeClaim("Usuarios.Ver")] 
    public async Task<IActionResult> Index(string? nombre, string? apellido, string? email, string? cuit, bool? habilitado, int page = 1, int pageSize = 10)
    {
        var users = await _usuarioManager.GetAllUsuarioModelsAsync();

        if (!string.IsNullOrEmpty(nombre))
        {
            users = users.Where(u => u.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (!string.IsNullOrEmpty(apellido))
        {
            users = users.Where(u => u.Apellido.Contains(apellido, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (!string.IsNullOrEmpty(email))
        {
            users = users.Where(u => u.Email.Contains(email, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        if (!string.IsNullOrEmpty(cuit))
        {
            cuit = new string(cuit.Where(char.IsDigit).ToArray()); // Remove non-numeric characters
            if (cuit.Length <= 11)
            {
                users = users.Where(u => u.Cuit.Contains(cuit)).ToList();
            }
        }
        if (habilitado.HasValue)
        {
            users = users.Where(u => u.Enabled == habilitado.Value).ToList();
        }

        // Order users by Apellido
        users = users.OrderBy(u => u.Apellido).ToList();

        var totalUsers = users.ToList().Count;
        var paginatedUsers = users.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

        return View(paginatedUsers);
    }

    [Authorize]
    [AuthorizeClaim("Roles.Ver")]
    public async Task<IActionResult> Roles(string? nombre)
    {
        var roles = await _rolManager.GetAllRolesAsync();

        if (!string.IsNullOrEmpty(nombre))
        {
            roles = roles.Where(r => r.Name.Contains(nombre, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return View(roles);
    }

    [Authorize]
    [AuthorizeClaim("Roles.Editar")]
    public async Task<IActionResult> AssignUsersToRole(string roleId)
    {
        var role = await _rolManager.GetRoleByIdAsync(roleId);
        if (role == null)
        {
            return View("NotFound");
        }

        var allUsers = await _usuarioManager.GetAllUsersAsync();
        var assignedUsers = await _usuarioManager.GetUsersInRoleAsync(roleId);

        var model = new AssignUsersToRoleViewModel
        {
            RoleId = roleId,
            RoleName = role.Name,
            AvailableUsers = allUsers.ExceptBy(assignedUsers.Select(u => u.Id), u => u.Id).ToList(),
            AssignedUsers = assignedUsers.ToList()
        };

        return View(model);
    }

    [HttpPost]
    [Authorize]
    [AuthorizeClaim("Roles.Editar")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignUsersToRole(AssignUsersToRoleViewModel model)
    {
        _logger.LogInformation("AssignUsersToRole POST called for RoleId: {RoleId}, RoleName: {RoleName}", model.RoleId, model.RoleName);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid: {@ModelState}", ModelState);
            return View(model);
        }

        try
        {
            // Retrieve all users and validate SelectedToAdd and SelectedToRemove
            var allUsers = await _usuarioManager.GetAllUsersAsync();
            var validUserIds = allUsers.Select(u => u.Id).ToHashSet();
            var invalidUserIds = model.SelectedToAdd?.Where(id => !validUserIds.Contains(id)).ToList();

            if (invalidUserIds != null && invalidUserIds.Any())
            {
                _logger.LogWarning("Invalid UserIds detected in SelectedToAdd: {InvalidUserIds}", string.Join(", ", invalidUserIds));
                ModelState.AddModelError("", "Algunos usuarios seleccionados no son válidos.");
                return View(model);
            }

            // Get the current assigned users before the update
            var currentAssignedUsers = await _usuarioManager.GetUsersInRoleAsync(model.RoleId);

            // Perform the update
            await _usuarioManager.UpdateUsersInRoleAsync(model.RoleId, model.SelectedToAdd, model.SelectedToRemove);

            // Get the updated assigned users after the update
            var updatedAssignedUsers = await _usuarioManager.GetUsersInRoleAsync(model.RoleId);

            // Determine the changes
            var addedUsers = updatedAssignedUsers.Where(u => !currentAssignedUsers.Any(cu => cu.Id == u.Id)).ToList();
            var removedUsers = currentAssignedUsers.Where(cu => !updatedAssignedUsers.Any(u => u.Id == cu.Id)).ToList();

            // Construct the success message
            var messageParts = new List<string>();
            if (addedUsers.Any())
            {
                var addedEmails = string.Join(", ", addedUsers.Select(u => u.Email));
                messageParts.Add($"Se incorporaron los siguientes usuarios: {addedEmails}.");
            }
            if (removedUsers.Any())
            {
                var removedEmails = string.Join(", ", removedUsers.Select(u => u.Email));
                messageParts.Add($"Se eliminaron los siguientes usuarios: {removedEmails}.");
            }

            TempData["SuccessMessage"] = $"El rol \"{model.RoleName}\" ha sido actualizado. {string.Join(" ", messageParts)}";

            return RedirectToAction("Roles");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while assigning users to role. RoleId: {RoleId}, RoleName: {RoleName}", model.RoleId, model.RoleName);
            TempData["ErrorMessage"] = "Ocurrió un error al asignar usuarios al rol.";
            return RedirectToAction("Generic", "Error");
        }
    }

    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult NoAutorizado()
    {
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
        if (result.Succeeded)
        {
            var user = await _usuarioManager.GetUserByEmailAsync(email);
            if (user != null)
            {
                // Use the claims generated by CustomClaimsPrincipalFactory
                var principal = await _signInManager.CreateUserPrincipalAsync(user);

                var claims = principal.Claims.ToList();

                // Add claim for full name
                var fullName = $"{user.Nombre} {user.Apellido}".Trim();
                claims.Add(new Claim("FullName", fullName));

                // Add claim for role description
                var roles = await _usuarioManager.GetRolesAsync(user);
                var roleDescription = roles.FirstOrDefault() ?? "Sin Rol";
                claims.Add(new Claim("RoleDescription", roleDescription));

                // Add any existing custom claims
                var vinculacion = await _personaManager.GetPersonaAsociadaAsync(user.Id);
                if (vinculacion != null)
                {
                    claims.Add(new System.Security.Claims.Claim("IdPersona", vinculacion.IdPersona.ToString()));
                }

                // Create new identity with all claims
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var nuevoPrincipal = new ClaimsPrincipal(identity);

                // Reauthenticate the user with the new identity
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, nuevoPrincipal);

                Console.WriteLine("User signed in successfully.");

                if (vinculacion == null)
                    return RedirectToAction("CreatePersonaUsuario", "Persona");

                return RedirectToAction("Index", "Home");
            }
        }

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
    [HttpPost]
    public async Task<IActionResult> Register(RegistroModel usuario)
    {
        try
        {
            if (ModelState.IsValid)
            {
                LicitArUser user = await _registroManager.RegistrarAsync(usuario, 1);
                if (user != null)
                {
                    return RedirectToAction("RegisterOk", "Usuario");

                }
            }

            ModelState.AddModelError("", "Error en el registro");
        }catch(Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
        }
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
    public IActionResult ConfirmarUsuario(string token, string userEmail)
    {
        TempData["Token"] = token;
        TempData["Email"] = userEmail;
        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ConfirmarUsuario(ConfirmarUsuarioModel model)
    {
     

        var result = await _signInManager.PasswordSignInAsync(model.email, model.password, false, false);
        if (result.Succeeded)
        {
            string decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.token));

            var confirmarUsuario = await _usuarioManager.ConfirmEmailAsync(decodedToken, model.email);

            if (confirmarUsuario)
            {
                return RedirectToAction("CreatePersonaUsuario", "Persona");
            }
        }

        ModelState.AddModelError("", "Login incorrecto");
        return View(model);
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
        if (user == null || !(_usuarioManager.IsEmailConfirmed(user)))
        {
            // Por seguridad, no revelamos si el usuario existe o si est� confirmado
            return RedirectToAction("ForgotPasswordConfirmation");
        }
        var result = await _registroManager.BlanquearPasswordAsync(model.Email);

        if (result != null)
        {
            TempData["SuccessMessage"] = "Se realiz� el blanqueo de la contrase�a, se envi� un link a su email";

        }


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
    public IActionResult ResetPassword(string token, string email)
    {
        if (token == null || email == null)
            return BadRequest("Faltan datos");

        return View(new ResetPasswordViewModel { Password = "", ConfirmPassword = "", Token = token, Email = email });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _registroManager.ResetPasswordAsync(model.Token, model.Email, model.Password);

        if (user != null)
        {
            TempData["SuccessMessage"] = "Se realiz� el blanqueo de la contrase�a de forma exitosa, por favor click en el siguiente link para iniciar sesi�n";

            return RedirectToAction("ResetPasswordOk");
        }

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ResetPasswordOk()
    {
        return View();
    }
    
    [AuthorizeClaim("Perfil.Ver")]
    public async Task<IActionResult> MyProfile()
    {
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            try
            {
                int idUsuario = IdentityHelper.GetUserLicitARId(User);
                var user = await _usuarioManager.GetUsuarioModelAsync(idUsuario);
                if (user == null)
                {
                    return View("NotFound");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MyProfile");
                throw;
            }
        }
        else
        {
            return View("NotFound");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Ensure Anti-Forgery Token validation
    [Authorize]
    [AuthorizeClaim("Perfil.Editar")]
    public async Task<IActionResult> EditMyProfile(UsuarioModel model)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("ModelState is invalid: {@ModelState}", ModelState);
            return View("MyProfile", model);
        }

        _logger.LogInformation("Updating user with ID: {IdUsuario}", model.IdUsuario);

        try
        {
            var user = await _usuarioManager.GetUserAsync(model.IdUsuario);
            if (user == null)
            {
                _logger.LogWarning("User with ID {IdUsuario} not found.", model.IdUsuario);
                return View("NotFound");
            }

            // Map UsuarioModel to LicitArUser
            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.Email = model.Email;
            user.FechaNacimiento = model.FechaNacimiento;
            user.Cuit = model.Cuit;

            var result = await _usuarioManager.UpdateUserAsync(user);
            if (!result)
            {
                _logger.LogError("Failed to update user with ID: {IdUsuario}", model.IdUsuario);
                ModelState.AddModelError("", "Error al actualizar el usuario.");
                return View("MyProfile", model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating user with ID: {IdUsuario}", model.IdUsuario);
            ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar el perfil.");
            return View("MyProfile", model);
        }

        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
        return RedirectToAction("MyProfile");
    }

    [HttpGet]
    [AuthorizeClaim("Usuarios.Editar")]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _usuarioManager.GetUsuarioModelAsync(id);
        if (user == null)
        {
            return View("NotFound");
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken] // Ensure AntiForgeryToken validation is enabled
    [Authorize]
    [AuthorizeClaim("Usuarios.Editar")]
    public async Task<IActionResult> Edit(UsuarioModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _logger.LogInformation("Updating user with ID: {IdUsuario}", model.IdUsuario);

        try
        {
            var user = await _usuarioManager.GetUserAsync(model.IdUsuario);
            if (user == null)
            {
                _logger.LogWarning("User with ID {IdUsuario} not found.", model.IdUsuario);
                return View("NotFound");
            }

            // Map UsuarioModel to LicitArUser
            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.Email = model.Email;
            user.FechaNacimiento = model.FechaNacimiento;
            user.Cuit = model.Cuit;

            var result = await _usuarioManager.UpdateUserAsync(user);
            if (!result)
            {
                _logger.LogError("Failed to update user with ID: {IdUsuario}", model.IdUsuario);
                ModelState.AddModelError("", "Error al actualizar el usuario.");
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating user with ID: {IdUsuario}", model.IdUsuario);
            ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar el usuario.");
            return View(model);
        }

        TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
        return RedirectToAction("Index");
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

    [HttpPost]
    [Authorize]
    [AuthorizeClaim("Usuarios.Eliminar")] 
    public async Task<IActionResult> ToggleEnabled(int id, bool enabled)
    {
        _logger.LogInformation("ToggleEnabled called with id: {id}, enabled: {enabled}", id, enabled);

        try
        {
            var result = await _usuarioManager.ToggleUserEnabledAsync(id, enabled);
            if (!result)
            {
                _logger.LogWarning("ToggleEnabled failed for id: {id}", id);
                return View("NotFound");
            }

            _logger.LogInformation("Successfully toggled Enabled property for user ID: {id} to {enabled}", id, enabled);
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while toggling Enabled property for user ID: {id}", id);
            return View("Error");
        }
    }
}
