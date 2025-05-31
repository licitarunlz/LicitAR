using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers.Authorization;
using Microsoft.Extensions.Logging;

namespace LicitAR.Web.Controllers
{
    public class LicitacionController : Controller
    {
        private readonly ILicitacionManager _licitacionManager;
        private readonly ILogger<LicitacionController> _logger;
        private readonly IOfertaManager _ofertaManager;
        private readonly IEvaluacionManager _evaluacionManager;

        public LicitacionController(ILicitacionManager licitacionManager, ILogger<LicitacionController> logger, IOfertaManager ofertaManager,
                                    IEvaluacionManager evaluacionManager)
        {
            _licitacionManager = licitacionManager;
            _logger = logger;
            _ofertaManager = ofertaManager;
            _evaluacionManager = evaluacionManager;
        }

        // GET: Licitacion
        [AuthorizeClaim("Licitaciones.Ver")]
        public async Task<IActionResult> Index(string codigoLicitacion, string titulo, DateTime? fechaPublicacion, DateTime? fechaCierre, int? idCategoriaLicitacion, int? idEstadoLicitacion, int page = 1, int pageSize = 10)
        {
            var licitacionesList = await _licitacionManager.GetAllLicitacionesAsync();
            var categorias = await _licitacionManager.GetAllCategoriasLicitacionAsync();
            var estados = await _licitacionManager.GetAllEstadosLicitacionAsync(); // NUEVO
            ViewBag.CategoriasLicitacion = categorias;
            ViewBag.EstadosLicitacion = estados; // NUEVO

            var query = licitacionesList.AsQueryable();

            if (!string.IsNullOrEmpty(codigoLicitacion))
            {
                query = query.Where(l => l.CodigoLicitacion != null && l.CodigoLicitacion.Contains(codigoLicitacion, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
            }

            if (fechaPublicacion.HasValue)
            {
                query = query.Where(l => l.FechaPublicacion.HasValue && l.FechaPublicacion.Value.Date == fechaPublicacion.Value.Date);
            }

            if (fechaCierre.HasValue)
            {
                query = query.Where(l => l.FechaCierre.HasValue && l.FechaCierre.Value.Date == fechaCierre.Value.Date);
            }

            if (idCategoriaLicitacion.HasValue)
            {
                query = query.Where(l => l.IdCategoriaLicitacion == idCategoriaLicitacion.Value);
            }

            if (idEstadoLicitacion.HasValue) // NUEVO
            {
                query = query.Where(l => l.IdEstadoLicitacion == idEstadoLicitacion.Value);
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
            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
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

            if (licitacion.IdEstadoLicitacion != 1)
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

            if (licitacion.IdEstadoLicitacion != 1)
            {
                return View("NotFound");
            }


            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
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

        // GET: Licitacion/Delete/5
        [AuthorizeClaim("Licitaciones.Editar")]
        public async Task<IActionResult> Publicar(int? id)
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

            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
            return View(licitacion);
        }

        // POST: Licitacion/Delete/5
        [HttpPost, ActionName("Publicar")]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Crear")]
        public async Task<IActionResult> PublicarConfirmed(LicitacionPublicarConfirmModel licitacion)
        {
            var result = await _licitacionManager.PublicarLicitacionAsync(licitacion.IdLicitacion, licitacion.FechaCierre, IdentityHelper.GetUserLicitARId(User));
            if (!result)
            {
                return View("NotFound"); // Updated
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Licitacion/Delete/5
        [AuthorizeClaim("Licitaciones.Crear")]
        public async Task<IActionResult> Evaluar(int? id)
        {

            

            if (id == null)
            {
                return View("NotFound"); // Updated
            }

            var evaluacionExistente = await _evaluacionManager.GetEvaluacionByLicitacionAsync(id.Value);
            if (evaluacionExistente != null)
                return RedirectToAction("Edit", "Evaluaciones", new { idEvaluacion = evaluacionExistente.IdEvaluacion });


            bool iniciarEvaluacion = await _licitacionManager.IniciarEvaluacionLicitacionAsync(id.Value, IdentityHelper.GetUserLicitARId(User));
            
            if (iniciarEvaluacion)
            {
                return RedirectToAction("Create", "Evaluaciones",new { idLicitacion = id});
            }

            return View("index");
        }

        // GET: Licitacion/Oferentes/5
        [AuthorizeClaim("Oferente.Ver")]
        public async Task<IActionResult> Offerer(int id)
        {
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id);
            if (licitacion == null)
            {
                return View("NotFound");
            }

            var oferentes = await _ofertaManager.GetOferentesPorLicitacionAsync(id);

            ViewBag.TituloLicitacion = licitacion.Titulo;
            ViewBag.RequisitosLicitacion = licitacion.Descripcion;

            return View(oferentes);
        }

        // GET: Licitacion/GetByEstado/5
        [AuthorizeClaim("Licitaciones.Ver")]
        public async Task<IActionResult> GetByEstado(int idEstadoLicitacion)
        {
            var licitaciones = await _licitacionManager.GetLicitacionesByEstadoAsync(idEstadoLicitacion);
            return View(licitaciones);
        }

        // GET: Licitacion/History/5
        public async Task<IActionResult> History(int id)
        {
            //_logger.LogInformation("Accediendo al historial de la licitación con ID {Id}", id);

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id);
            if (licitacion == null)
            {
                _logger.LogWarning("No se encontró la licitación con ID {Id}", id);
                return View("NotFound");
            }

            var historial = await _licitacionManager.GetHistorialEstados(id);

            ViewBag.Licitacion = licitacion;
            //_logger.LogInformation("Historial obtenido para la licitación con ID {Id}. Estados encontrados: {Count}", id, historial?.Count ?? 0);
            return View(historial);
        }
    }
}
