using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Utils;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers;
using LicitAR.Web.Helpers.Authorization;

namespace LicitAR.Web.Controllers
{
    public class PersonaController : Controller
    {
        private readonly IPersonaManager _personaManager;
        private readonly LicitARDbContext _context;
        private IMessageManager _messageManager;

        public PersonaController(LicitARDbContext context, IPersonaManager personaManager, IMessageManager messageManager)
        {
            _personaManager = personaManager;
            _context = context;
            _messageManager = messageManager;
        }

        // GET: Persona
        [AuthorizeClaim("Persona.Ver")]
        public async Task<IActionResult> Index()
        {
            var personas = await _personaManager.GetAllPersonasAsync();
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

        // POST: Persona/Create
        [AuthorizeClaim("Persona.Crear")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var items = _context.Provincias
                   .Select(x => new SelectListItem
                   {
                       Value = x.IdProvincia.ToString(),
                       Text = x.Descripcion
                   })
                   .ToList();
            var itemsLocalidades = _context.Localidades.ToList();

            ViewBag.ComboProvincias = items;
            ViewBag.ComboLocalidades = itemsLocalidades;

            var personaModel = new PersonaModel();
            personaModel.SetPersonaData(persona);

            return View(personaModel);
        }

        // POST: Persona/Edit/5
        [AuthorizeClaim("Persona.Editar")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var persona = await _context.Personas.FindAsync(id);
            if (persona != null)
            {
                _context.Personas.Remove(persona);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonaExists(int id)
        {
            return _context.Personas.Any(e => e.IdPersona == id);
        }
    }
}
