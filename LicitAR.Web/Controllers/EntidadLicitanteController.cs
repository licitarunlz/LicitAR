using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Models;
using LicitAR.Core.Utils;
using LicitAR.Web.Helpers;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Web.Helpers.Authorization;

namespace LicitAR.Web.Controllers
{
    public class EntidadLicitanteController : Controller
    {
        private readonly ActoresDbContext _context;
        private readonly IEntidadLicitanteManager _entidadLicitanteManager;
        private readonly ParametrosDbContext _parametrosDbContext;
        private IMessageManager _messageManager;

        public EntidadLicitanteController(ActoresDbContext context, ParametrosDbContext parametrosDbContext, IEntidadLicitanteManager entidadLicitanteManager, IMessageManager message)
        {
            _context = context;
            _entidadLicitanteManager = entidadLicitanteManager;
            _parametrosDbContext = parametrosDbContext;
            _messageManager = message;
        }

        // GET: EntidadLicitante
        [AuthorizeClaim("EntidadLicitante.Ver")]
        public IActionResult Index(string cuit, string razonSocial, int page = 1, int pageSize = 10)
        {
            var query = _context.EntidadesLicitantes.AsQueryable();

            if (!string.IsNullOrEmpty(cuit))
            {
                query = query.Where(e => e.Cuit.Contains(cuit));
            }

            if (!string.IsNullOrEmpty(razonSocial))
            {
                query = query.Where(e => e.RazonSocial.Contains(razonSocial));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        // GET: EntidadLicitante/Details/5
        [AuthorizeClaim("EntidadLicitante.Ver")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _context.EntidadesLicitantes
                .FirstOrDefaultAsync(m => m.IdEntidadLicitante == id);
            if (entidadLicitante == null)
            {
                return NotFound();
            }

            return View(entidadLicitante);
        }

        // GET: EntidadLicitante/Create
        [AuthorizeClaim("EntidadLicitante.Crear")]
        public IActionResult Create()
        {
            var items = _parametrosDbContext.Provincias
                    .Select(x => new SelectListItem
                    {
                        Value = x.IdProvincia.ToString(),
                        Text = x.Descripcion
                    })
                    .ToList();
            var itemsLocalidades = _parametrosDbContext.Localidades.ToList();
                  
            ViewBag.ComboProvincias = items;
            ViewBag.ComboLocalidades = itemsLocalidades;


            return View();
        }

        // POST: EntidadLicitante/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("EntidadLicitante.Crear")]
        public async Task<IActionResult> Create(EntidadLicitanteModel entidadLicitanteModel)
        {
            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                EntidadLicitante entidadLicitante = entidadLicitanteModel.GetEntidadLicitante(audit);

                this._messageManager = await _entidadLicitanteManager.AgregarAsync(entidadLicitante, IdentityHelper.GetUserLicitARId(User));

                ViewBag.Messages = _messageManager.messages;

                if (!_messageManager.HasErrors)
                {
                    return RedirectToAction(nameof(Index));

                }else
                {
                    return View(entidadLicitanteModel);
                }
            }
            return View(entidadLicitanteModel);
        }

        // GET: EntidadLicitante/Edit/5
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _entidadLicitanteManager.GetEntidadLicitanteByIdAsync(id.Value);
            if (entidadLicitante == null)
            {
                return NotFound();
            }
            EntidadLicitanteModel ent = new EntidadLicitanteModel();
            ent.SetEntidadLicitanteData(entidadLicitante);
            return View(ent);
        }

        // POST: EntidadLicitante/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> Edit(int id, EntidadLicitanteModel entidadLicitante)
        {
            if (id != entidadLicitante.IdEntidadLicitante)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entidad = entidadLicitante.GetEntidadLicitante(AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User)));
                    entidad.IdEntidadLicitante = entidadLicitante.IdEntidadLicitante;

                    _messageManager = await _entidadLicitanteManager.ModificarAsync(entidad, id, IdentityHelper.GetUserLicitARId(User));
                }
                catch (Exception ex)
                {
                    _messageManager.ErrorMessage("Excepcion al intentar modificar una licitaciòn: Ex - " + ex.ToString());
                }
                finally
                {
                    ViewBag.messages = _messageManager.messages;
                }

                if (_messageManager.HasErrors)
                    return View(entidadLicitante);
                else 
                    return RedirectToAction(nameof(Index));
            }
            return View(entidadLicitante);
        }

        // GET: EntidadLicitante/Delete/5
        [AuthorizeClaim("EntidadLicitante.Eliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _context.EntidadesLicitantes
                .FirstOrDefaultAsync(m => m.IdEntidadLicitante == id);
            if (entidadLicitante == null)
            {
                return NotFound();
            }

            return View(entidadLicitante);
        }

        // POST: EntidadLicitante/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("EntidadLicitante.Eliminar")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            _messageManager = await _entidadLicitanteManager.BajaLogicaAsync(id, IdentityHelper.GetUserLicitARId(User));

            ViewBag.messages = _messageManager.messages;

            if (_messageManager.HasErrors)
                return View(id);
            else
                return RedirectToAction(nameof(Index));
        }

        private bool EntidadLicitanteExists(int id)
        {
            return _context.EntidadesLicitantes.Any(e => e.IdEntidadLicitante == id);
        }
    }
}
