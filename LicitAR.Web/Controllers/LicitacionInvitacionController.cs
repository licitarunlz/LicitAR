using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Web.Models;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using System.Linq;
using LicitAR.Core.Business.Auditoria;
using LicitAR.Web.Helpers.Auditoria;
using Microsoft.Extensions.Configuration;

namespace LicitAR.Web.Controllers
{
    public class LicitacionInvitacionController : Controller
    {
        private readonly ILicitacionInvitacionManager _manager;
        private readonly IPersonaManager _personaManager;
        private readonly ILicitacionManager _licitacionManager;
        private readonly IAuditManager _auditManager;
        private readonly IConfiguration _config;

        public LicitacionInvitacionController(
            ILicitacionInvitacionManager manager,
            IPersonaManager personaManager,
            ILicitacionManager licitacionManager,
            IAuditManager auditManager,
            IConfiguration config)
        {
            _manager = manager;
            _personaManager = personaManager;
            _licitacionManager = licitacionManager;
            _auditManager = auditManager;
            _config = config;
        }

        [AuditarEvento("LicitacionInvitacionController - Tabla", "LicitacionInvitacion", "Visualización de invitaciones de licitación")]
        public async Task<IActionResult> Index(int? idLicitacion, string codigoLicitacion, string cuit, string razonSocial, int page = 1, int pageSize = 10)
        {
            List<LicitacionInvitacion> invitaciones;
            if (idLicitacion.HasValue)
                invitaciones = await _manager.GetByLicitacionAsync(idLicitacion.Value);
            else
                invitaciones = await _manager.GetAllAsync();

            var query = invitaciones.AsQueryable();

            if (!string.IsNullOrEmpty(codigoLicitacion))
            {
                var codigoLower = codigoLicitacion.ToLowerInvariant();
                query = query.Where(x => x.Licitacion.CodigoLicitacion != null && x.Licitacion.CodigoLicitacion.ToLowerInvariant().Contains(codigoLower));
            }

            if (!string.IsNullOrEmpty(cuit))
            {
                var cuitNumbers = new string(cuit.Where(char.IsDigit).ToArray());
                query = query.Where(x => x.Persona.Cuit != null && x.Persona.Cuit.Contains(cuitNumbers));
            }

            if (!string.IsNullOrEmpty(razonSocial))
            {
                var razonLower = razonSocial.ToLowerInvariant();
                query = query.Where(x => x.Persona.RazonSocial != null && x.Persona.RazonSocial.ToLowerInvariant().Contains(razonLower));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paged = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = paged.Select(x => new LicitacionInvitacionModel
            {
                IdLicitacion = x.IdLicitacion,
                CodigoLicitacion = x.Licitacion.CodigoLicitacion,
                TituloLicitacion = x.Licitacion.Titulo,
                IdPersona = x.IdPersona,
                Cuit = x.Persona.Cuit,
                RazonSocial = x.Persona.RazonSocial,
                FechaInvitacion = x.FechaInvitacion
            }).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.IdLicitacion = idLicitacion;

            if (idLicitacion.HasValue)
            {
                var lic = invitaciones.FirstOrDefault()?.Licitacion;
                if (lic != null)
                {
                    ViewBag.CodigoLicitacion = lic.CodigoLicitacion;
                    ViewBag.TituloLicitacion = lic.Titulo;
                }
            }
            else
            {
                ViewBag.CodigoLicitacion = null;
                ViewBag.TituloLicitacion = null;
            }

            return View(model);
        }

        [HttpPost]
        [AuditarEvento("LicitacionInvitacionController - Eliminar", "LicitacionInvitacion", "Eliminación de invitación de persona a licitación")]
        public async Task<IActionResult> Remove(int idLicitacion, int idPersona)
        {
            await _manager.RemoveInvitacionAsync(idLicitacion, idPersona);
            return RedirectToAction(nameof(Index), new { idLicitacion });
        }

        // GET: LicitacionInvitacion/AssignPersonaToLicitacion/5
        public async Task<IActionResult> AssignPersonaToLicitacion(int idLicitacion)
        {
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(idLicitacion);
            var personas = await _personaManager.GetAllPersonasAsync();

            var invitadas = await _manager.GetByLicitacionAsync(idLicitacion);
            var assignedIds = invitadas.Select(x => x.IdPersona).ToHashSet();

            var model = new AssignPersonaToLicitacionViewModel
            {
                IdLicitacion = idLicitacion,
                CodigoLicitacion = licitacion.CodigoLicitacion,
                TituloLicitacion = licitacion.Titulo,
                AvailablePersonas = personas.Where(p => !assignedIds.Contains(p.IdPersona)).ToList(),
                AssignedPersonas = personas.Where(p => assignedIds.Contains(p.IdPersona)).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [AuditarEvento("LicitacionInvitacionController - Asignar Persona", "LicitacionInvitacion", "Asignación de personas a licitación")]
        public async Task<IActionResult> AssignPersonaToLicitacion(AssignPersonaToLicitacionViewModel model, List<int> SelectedToAdd, List<int> SelectedToRemove)
        {
            int idUsuario = IdentityHelper.GetUserLicitARId(User);

            if (SelectedToAdd != null && SelectedToAdd.Any())
            {
                var baseUrl = _config["App:BaseUrl"] ?? "https://tusitio.com";
                await _manager.AddInvitacionesAsync(model.IdLicitacion, SelectedToAdd, idUsuario, baseUrl);
                foreach (var idPersona in SelectedToAdd)
                {
                    var persona = await _personaManager.GetPersonaByIdAsync(idPersona);
                    await _auditManager.LogLicitacionChange(
                        model.IdLicitacion,
                        idUsuario,
                        "Invitación Persona Agregada",
                        "Persona",
                        null,
                        persona?.RazonSocial ?? $"Id:{idPersona}"
                    );
                }
            }

            if (SelectedToRemove != null && SelectedToRemove.Any())
            {
                foreach (var idPersona in SelectedToRemove)
                {
                    await _manager.RemoveInvitacionAsync(model.IdLicitacion, idPersona);
                    var persona = await _personaManager.GetPersonaByIdAsync(idPersona);
                    await _auditManager.LogLicitacionChange(
                        model.IdLicitacion,
                        idUsuario,
                        "Invitación Persona Eliminada",
                        "Persona",
                        persona?.RazonSocial ?? $"Id:{idPersona}",
                        null
                    );
                }
            }

            return RedirectToAction("Index", new { idLicitacion = model.IdLicitacion });
        }
    }

}
