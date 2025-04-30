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
              
                await _licitacionManager.CreateLicitacionAsync(licitacion, IdentityHelper.GetUserLicitARId(User));
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
            var lic = new LicitacionModel();
            lic.SetLicitacionData(licitacion);
            return View( lic);
        }

        // POST: Licitacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LicitacionModel licitacionModel)
        {
            if (id != licitacionModel.IdLicitacion) // Fix comparison to match IdLicitacion
            {
                return View("NotFound"); // Updated
            }

            if (ModelState.IsValid)
            {
                var licitacion = licitacionModel.GetLicitacion(AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User)));
                licitacion.IdLicitacion = licitacionModel.IdLicitacion;

                var result = await _licitacionManager.UpdateLicitacionAsync(licitacion, IdentityHelper.GetUserLicitARId(User));
                if (!result)
                {
                    return View("NotFound"); // Updated
                }
                return RedirectToAction(nameof(Index));
            }
            return View(licitacionModel);
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
            var result = await _licitacionManager.DeleteLicitacionAsync(id, IdentityHelper.GetUserLicitARId(User));
            if (!result)
            {
                return View("NotFound"); // Updated
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Licitacion/Oferentes/5
        public async Task<IActionResult> Offerer(int id)
        {
            // Obtener la licitación desde la base de datos
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id);
            if (licitacion == null)
            {
                return View("NotFound"); // Manejar caso de licitación no encontrada
            }

            // Simulación de datos de oferentes
            var oferentes = new List<OferenteModel>
            {
                new OferenteModel { Id = 1, IdLicitacion = id, TituloLicitacion = licitacion.Titulo, RequisitosLicitacion = licitacion.Descripcion, NombreEntidad = "Entidad A", Fecha = DateTime.Now.AddDays(-2), CumpleRequisitos = true },
                new OferenteModel { Id = 2, IdLicitacion = id, TituloLicitacion = licitacion.Titulo, RequisitosLicitacion = licitacion.Descripcion, NombreEntidad = "Entidad B", Fecha = DateTime.Now.AddDays(-1), CumpleRequisitos = false },
                new OferenteModel { Id = 3, IdLicitacion = id, TituloLicitacion = licitacion.Titulo, RequisitosLicitacion = licitacion.Descripcion, NombreEntidad = "Entidad C", Fecha = DateTime.Now, CumpleRequisitos = true }
            };

            return View(oferentes);
        }
    }
}
