using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers.Authorization;

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
        [AuthorizeClaim("Licitaciones.Ver")]
        public async Task<IActionResult> Index(string codigoLicitacion, string titulo, DateTime? fechaPublicacion, DateTime? fechaCierre, int? idCategoriaLicitacion, int page = 1, int pageSize = 10)
        {
            var licitacionesList = await _licitacionManager.GetAllLicitacionesAsync();
            var categorias = await _licitacionManager.GetAllCategoriasAsync();
            ViewBag.CategoriasLicitacion = categorias;

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

            if (idCategoriaLicitacion.HasValue)
            {
                query = query.Where(l => l.IdCategoriaLicitacion == idCategoriaLicitacion.Value);
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
        [AuthorizeClaim("Licitaciones.Ver")]
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
        [AuthorizeClaim("Licitaciones.Crear")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Licitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Crear")]
        public async Task<IActionResult> Create(LicitacionModel licitacionModel)
        {
            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                var estadoLicitacion = await _licitacionManager.GetEstadoLicitacionByIdAsync(licitacionModel.IdEstadoLicitacion);
                var categoriaLicitacion = await _licitacionManager.GetCategoriaLicitacionByIdAsync(licitacionModel.IdCategoriaLicitacion);

                Licitacion licitacion = licitacionModel.GetLicitacion(audit, estadoLicitacion, categoriaLicitacion);
                licitacion.Items = licitacionModel.GetLicitacionDetalles(audit);
                await _licitacionManager.CreateLicitacionAsync(licitacion, IdentityHelper.GetUserLicitARId(User));
                return RedirectToAction(nameof(Index));
            }
            return View(licitacionModel);
        }

        // GET: Licitacion/Edit/5
        [AuthorizeClaim("Licitaciones.Editar")]
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
            return View(lic);
        }

        // POST: Licitacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Editar")]
        public async Task<IActionResult> Edit(int id, LicitacionModel licitacionModel)
        {
            if (id != licitacionModel.IdLicitacion) // Fix comparison to match IdLicitacion
            {
                return View("NotFound"); // Updated
            }

            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                var estadoLicitacion = await _licitacionManager.GetEstadoLicitacionByIdAsync(licitacionModel.IdEstadoLicitacion);
                var categoriaLicitacion = await _licitacionManager.GetCategoriaLicitacionByIdAsync(licitacionModel.IdCategoriaLicitacion);

                var licitacion = licitacionModel.GetLicitacion(audit, estadoLicitacion, categoriaLicitacion);
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
        [AuthorizeClaim("Licitaciones.Eliminar")]
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
        [AuthorizeClaim("Licitaciones.Eliminar")]
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
        [AuthorizeClaim("Oferente.Ver")]
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

        // GET: Licitacion/GetByEstado/5
        [AuthorizeClaim("Licitaciones.Ver")]
        public async Task<IActionResult> GetByEstado(int idEstadoLicitacion)
        {
            var licitaciones = await _licitacionManager.GetLicitacionesByEstadoAsync(idEstadoLicitacion);
            return View(licitaciones);
        }
    }
}
