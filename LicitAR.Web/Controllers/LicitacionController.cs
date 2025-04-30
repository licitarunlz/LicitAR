using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Web.Models;

namespace LicitAR.Web.Controllers
{
    public class LicitacionController : Controller
    {
        private readonly ILicitacionManager _licitacionManager;

        public LicitacionController(ILicitacionManager licitacionManager) // Fixed syntax issue
        {
            _licitacionManager = licitacionManager;
        }

        // GET: Licitacion
        public async Task<IActionResult> Index(string codigoLicitacion, string titulo, DateTime? fechaPublicacion, DateTime? fechaCierre, int page = 1, int pageSize = 10)
        {
            var licitacionesList = await _licitacionManager.GetAllLicitacionesAsync();
            var query = licitacionesList.AsQueryable();

            if (!string.IsNullOrEmpty(codigoLicitacion))
            {
                query = query.Where(l => l.CodigoLicitacion.Contains(codigoLicitacion));
            }

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Titulo.Contains(titulo));
            }

            if (fechaPublicacion.HasValue)
            {
                query = query.Where(l => l.FechaPublicacion.Date == fechaPublicacion.Value.Date);
            }

            if (fechaCierre.HasValue)
            {
                query = query.Where(l => l.FechaCierre.Date == fechaCierre.Value.Date);
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var licitaciones = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(licitaciones);
        }

        // GET: Licitacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("NotFound"); // Updated
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id.Value);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }

            return View(licitacion);
        }

        // GET: Licitacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Licitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( LicitacionModel licitacionModel)
        {
            

            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                Licitacion licitacion = licitacionModel.GetLicitacion(audit);
                licitacion.Audit = audit;
                await _licitacionManager.CreateLicitacionAsync(licitacion);
                return RedirectToAction(nameof(Index));
            }
            return View(licitacionModel);
        }

        // GET: Licitacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound"); 
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id.Value);
            if (licitacion == null)
            {
                return View("NotFound"); 
            }

            return View(licitacion);
        }

        // POST: Licitacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLicitacion,IdEntidadLicitante,CodigoLicitacion,Titulo,Descripcion,FechaPublicacion,FechaCierre,IdEstadoLicitacion,IdCategoriaLicitacion")] Licitacion licitacion)
        {
            if (id != licitacion.IdLicitacion) // Fix comparison to match IdLicitacion
            {
                return View("NotFound"); // Updated
            }

            if (ModelState.IsValid)
            {
                var result = await _licitacionManager.UpdateLicitacionAsync(licitacion);
                if (!result)
                {
                    return View("NotFound"); // Updated
                }
                return RedirectToAction(nameof(Index));
            }
            return View(licitacion);
        }

        // GET: Licitacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound"); // Updated
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id.Value);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }

            return View(licitacion);
        }

        // POST: Licitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _licitacionManager.DeleteLicitacionAsync(id);
            if (!result)
            {
                return View("NotFound"); // Updated
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
