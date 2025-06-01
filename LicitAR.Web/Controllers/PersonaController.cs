using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Utils;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers;
using LicitAR.Web.Helpers.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using LicitAR.Core.Data.Models.Identidad;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using LicitAR.Core.Business.Identidad;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using LicitAR.Web.Helpers.Auditoria;

namespace LicitAR.Web.Controllers
{

    public class PersonaController : Controller
    {
        private readonly IPersonaManager _personaManager;
        private readonly LicitARDbContext _context;
        private IMessageManager _messageManager;
        private SignInManager<LicitArUser> _signInManager;
        private IUsuarioManager _usuarioManager;

        public PersonaController(LicitARDbContext context, IPersonaManager personaManager, IMessageManager messageManager, SignInManager<LicitArUser> signInManager, IUsuarioManager usuarioManager)
        {
            _personaManager = personaManager;
            _context = context;
            _messageManager = messageManager;
            _signInManager = signInManager;
            _usuarioManager = usuarioManager;
        }

        // GET: Persona
        [AuthorizeClaim("Persona.Ver")]
        public async Task<IActionResult> Index(string cuit, string razonSocial, int page = 1, int pageSize = 10)
        {
            var personasList = await _personaManager.GetAllPersonasAsync();
             
            var query = personasList.AsQueryable();

            if (!string.IsNullOrEmpty(cuit))
            {
                cuit = new string(cuit.Where(char.IsDigit).ToArray()); // Remove non-numeric characters
                if (cuit.Length <= 11)
                {
                    query = query.Where(l => l.Cuit.Contains(cuit));
                }
            }

            if (!string.IsNullOrEmpty(razonSocial))
            {
                query = query.Where(l => l.RazonSocial.Contains(razonSocial, StringComparison.OrdinalIgnoreCase));
            }
 

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var personas = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            List<PersonaModel> listPersonaModel = new List<PersonaModel>();

            foreach (var persona in personas)
            {
                var personaModel = new PersonaModel();
                personaModel.SetPersonaData(persona);
                listPersonaModel.Add(personaModel);
            }

            return View(listPersonaModel);
        }

        // GET: Persona/Details/5
        [AuthorizeClaim("Persona.Ver")]
        [AuditarEvento("PersonaController - Details", "Persona", "Visualización de detalle de persona", "id")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _personaManager.GetPersonaByIdAsync(id.Value);
            if (persona == null)
            {
                return NotFound();
            }
            var personaModel = new PersonaModel();
            personaModel.SetPersonaData(persona);
            return View(personaModel);
        }

        // GET: Persona/Create
        [AuthorizeClaim("Persona.Crear")]
        [AuditarEvento("PersonaController - Create", "Persona", "Inicio creación de persona")]
        public IActionResult Create()
        {
            var items = _context.Provincias
                   .Select(x => new SelectListItem
                   {
                       Value = x.IdProvincia.ToString(),
                       Text = x.Descripcion
                   })
                   .ToList();
            var itemsLocalidades = _context.Localidades.ToList();

            var tiposPersonas = _context.TiposPersona
                    .Select(x => new SelectListItem 
                    { 
                        Value = x.IdTipoPersona.ToString(), 
                        Text = x.Descripcion.ToString() 
                    }).ToList();

            ViewBag.ComboProvincias = items;
            ViewBag.ComboLocalidades = itemsLocalidades;
            ViewBag.ComboTiposPersona = tiposPersonas;
            return View();
        }
        // GET: Persona/CreatePersonaUsuario
        [AuthorizeClaim("Persona.Crear")]
        [AuditarEvento("PersonaController - CreatePersonaUsuario", "Persona", "Inicio creación de persona usuario")]
        public IActionResult CreatePersonaUsuario()
        {
            PersonaModel personaModel = new PersonaModel();

            var user = _context.Users.FirstOrDefault(x => x.IdUsuario == IdentityHelper.GetUserLicitARId(User));

            if (user == null)
                return NotFound();

            personaModel.Cuit = user.Cuit;
            personaModel.Email = user.Email;
            personaModel.RazonSocial = user.Apellido + ", " + user.Nombre;
            
            var items = _context.Provincias
                   .Select(x => new SelectListItem
                   {
                       Value = x.IdProvincia.ToString(),
                       Text = x.Descripcion
                   })
                   .ToList();
            var itemsLocalidades = _context.Localidades.ToList();

            var tiposPersonas = _context.TiposPersona
                    .Select(x => new SelectListItem
                    {
                        Value = x.IdTipoPersona.ToString(),
                        Text = x.Descripcion.ToString()
                    }).ToList();

            ViewBag.ComboProvincias = items;
            ViewBag.ComboLocalidades = itemsLocalidades;
            ViewBag.ComboTiposPersona = tiposPersonas;
            return View(personaModel);
        }

        // POST: Persona/CreatePersonaUsuario
        [AuthorizeClaim("Persona.Crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditarEvento("PersonaController - CreatePersonaUsuario", "Persona", "Creación de persona usuario")]
        public async Task<IActionResult> CreatePersonaUsuario(PersonaModel personaModel)
        {
            if (ModelState.IsValid)
            {
                int idUser = IdentityHelper.GetUserLicitARId(User);
                string user = IdentityHelper.GetUserLicitARGuid(User);


                var audit = AuditHelper.GetCreationData(idUser);
                Persona persona = personaModel.GetPersona(audit);

                this._messageManager = await _personaManager.AgregarAsync(persona, idUser);

                this._messageManager = await _personaManager.AsociarUsuarioAsync(persona, user, idUser);

                ViewBag.Messages = _messageManager.messages;

                if (!_messageManager.HasErrors)
                {

                    /*var userDdbb = await _usuarioManager.GetUserByEmailAsync(personaModel.Email);
                    // Use the claims generated by CustomClaimsPrincipalFactory
                    var principal = await _signInManager.CreateUserPrincipalAsync(userDdbb);
                    principal.Claims.add)*/

                    // Suponé que ya tenés el usuario autenticado
                    var existingPrincipal = HttpContext.User;

                    // Clonás las claims existentes
                    var claims = existingPrincipal.Claims.ToList();

                    // Agregás la nueva claim
                    claims.Add(new Claim("IdPersona", ((PersonaUsuario)_messageManager.addedData).IdPersona.ToString() ));

                    // Creamos nuevo identity y principal
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                    return RedirectToAction("Index","Home");

                }
                else
                {
                    return View(personaModel);
                }
            }
            return View(personaModel);
        }


        // POST: Persona/Create
        [AuthorizeClaim("Persona.Crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditarEvento("PersonaController - Create", "Persona", "Creación de persona")]
        public async Task<IActionResult> Create(PersonaModel personaModel)
        {
            if (ModelState.IsValid)
            {
                int idUser = IdentityHelper.GetUserLicitARId(User);
                var audit = AuditHelper.GetCreationData(idUser);
                Persona persona = personaModel.GetPersona(audit);

                this._messageManager = await _personaManager.AgregarAsync(persona, idUser);

                ViewBag.Messages = _messageManager.messages;

                if (!_messageManager.HasErrors)
                {
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    return View(personaModel);
                }
            }
            return View(personaModel);
        }

        // GET: Persona/Edit/5
        [AuthorizeClaim("Persona.Editar")]
        [AuditarEvento("PersonaController - Edit", "Persona", "Inicio edición de persona", "id")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _personaManager.GetPersonaByIdAsync(id.Value);

            if (persona == null)
            {
                return NotFound();
            }
            var tiposPersonas = _context.TiposPersona
                 .Select(x => new SelectListItem
                 {
                     Value = x.IdTipoPersona.ToString(),
                     Text = x.Descripcion.ToString()
                 }).ToList();

            var items = _context.Provincias
                   .Select(x => new SelectListItem
                   {
                       Value = x.IdProvincia.ToString(),
                       Text = x.Descripcion
                   })
                   .ToList();
            var itemsLocalidades = _context.Localidades
                                            .Where(x=> x.IdProvincia == persona.IdProvincia)
                                            .Select(x => new SelectListItem
                                            {
                                                Value = x.IdLocalidad.ToString(),
                                                Text = x.Descripcion
                                            }).ToList();

            ViewBag.ComboProvincias = items;
            ViewBag.ComboLocalidades = itemsLocalidades;
            ViewBag.ComboTiposPersona = tiposPersonas;


            var personaModel = new PersonaModel();
            personaModel.SetPersonaData(persona);

            return View(personaModel);
        }

        // POST: Persona/Edit/5
        [AuthorizeClaim("Persona.Editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditarEvento("PersonaController - Edit", "Persona", "Edición de persona", "id")]
        public async Task<IActionResult> Edit(int id, PersonaModel personaModel)
        {
            if (id != personaModel.IdPersona)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    int idUser = IdentityHelper.GetUserLicitARId(User);
                    var entidad = personaModel.GetPersona(AuditHelper.GetCreationData(idUser));
                    entidad.IdPersona = personaModel.IdPersona;

                    _messageManager = await _personaManager.ModificarAsync(entidad, id, idUser);
                }
                catch (Exception ex)
                {
                    _messageManager.ErrorMessage("Excepcion al intentar modificar una Persona: Ex - " + ex.ToString());
                }
                finally
                {
                    ViewBag.messages = _messageManager.messages;
                }

                if (_messageManager.HasErrors)
                    return View(personaModel);
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(personaModel);
        }

        // GET: Persona/Delete/5
        [AuthorizeClaim("Persona.Eliminar")]
        [AuditarEvento("PersonaController - Delete", "Persona", "Inicio eliminación de persona", "id")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var persona = await _personaManager.GetPersonaByIdAsync(id.Value);
            if (persona == null)
            {
                return NotFound();
            }

            PersonaModel personaModel = new PersonaModel();
            personaModel.SetPersonaData(persona);

            return View(personaModel);
        }

        // POST: Persona/Delete/5
        [AuthorizeClaim("Persona.Eliminar")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuditarEvento("PersonaController - DeleteConfirmed", "Persona", "Confirmación de eliminación de persona", "id")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int idUser = IdentityHelper.GetUserLicitARId(User);
            var result = await _personaManager.BajaLogicaAsync(id, idUser);

            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.IdPersona == id);
        }
    }
}
