using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Web.Models;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using System.Linq;

namespace LicitAR.Web.Controllers
{
    public class LicitacionInvitacionController : Controller
    {
        private readonly ILicitacionInvitacionManager _manager;
        private readonly IPersonaManager _personaManager;
        private readonly ILicitacionManager _licitacionManager;

        public LicitacionInvitacionController(ILicitacionInvitacionManager manager, IPersonaManager personaManager, ILicitacionManager licitacionManager)
        {
            _manager = manager;
            _personaManager = personaManager;
            _licitacionManager = licitacionManager;
        }

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
        public async Task<IActionResult> AssignPersonaToLicitacion(AssignPersonaToLicitacionViewModel model, List<int> SelectedToAdd, List<int> SelectedToRemove)
        {
            int idUsuario = IdentityHelper.GetUserLicitARId(User);

            if (SelectedToAdd != null && SelectedToAdd.Any())
                await _manager.AddInvitacionesAsync(model.IdLicitacion, SelectedToAdd, idUsuario);

            if (SelectedToRemove != null && SelectedToRemove.Any())
            {
                foreach (var idPersona in SelectedToRemove)
                    await _manager.RemoveInvitacionAsync(model.IdLicitacion, idPersona);
            }

            return RedirectToAction("Index", new { idLicitacion = model.IdLicitacion });
        }
    }

    public class AssignPersonaToLicitacionViewModel
    {
        public int IdLicitacion { get; set; }
        public string CodigoLicitacion { get; set; }
        public string TituloLicitacion { get; set; }
        public List<Persona> AvailablePersonas { get; set; }
        public List<Persona> AssignedPersonas { get; set; }
    }
}
