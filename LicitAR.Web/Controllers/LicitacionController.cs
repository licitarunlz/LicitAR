using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers.Authorization;
using Microsoft.Extensions.Logging;
using LicitAR.Core.Data;
using LicitAR.Core.Business.Auditoria;
using LicitAR.Web.Helpers.Auditoria;

namespace LicitAR.Web.Controllers
{
    public class LicitacionController : Controller
    {
        private readonly ILicitacionManager _licitacionManager;
        private readonly ILogger<LicitacionController> _logger;
        private readonly IOfertaManager _ofertaManager;
        private readonly IEvaluacionManager _evaluacionManager;
        private readonly LicitARDbContext _dbContext;
        private readonly IAuditManager _auditManager;

        public LicitacionController(
            ILicitacionManager licitacionManager,
            ILogger<LicitacionController> logger,
            IOfertaManager ofertaManager,
            IEvaluacionManager evaluacionManager,
            LicitARDbContext dbContext,
            IAuditManager auditManager)
        {
            _licitacionManager = licitacionManager;
            _logger = logger;
            _ofertaManager = ofertaManager;
            _evaluacionManager = evaluacionManager;
            _dbContext = dbContext;
            _auditManager = auditManager;
        }

        private static string FormatearCuit(string cuit)
        {
            if (string.IsNullOrEmpty(cuit) || cuit.Length != 11)
                return cuit;
            return $"{cuit.Substring(0, 2)}-{cuit.Substring(2, 8)}-{cuit.Substring(10, 1)}";
        }

        private static string FormatearCuitSeguro(string cuit, string razonSocial)
        {
            if (string.IsNullOrWhiteSpace(cuit) && string.IsNullOrWhiteSpace(razonSocial))
                return "[S/D] - S/D";

            string cuitFormateado;
            if (string.IsNullOrWhiteSpace(cuit))
            {
                cuitFormateado = "S/D";
            }
            else if (cuit.Length == 11 && long.TryParse(cuit, out _))
            {
                cuitFormateado = $"{cuit.Substring(0, 2)}-{cuit.Substring(2, 8)}-{cuit.Substring(10, 1)}";
            }
            else
            {
                cuitFormateado = cuit;
            }

            string razon = string.IsNullOrWhiteSpace(razonSocial) ? "S/D" : razonSocial;

            return $"[{cuitFormateado}] - {razon}";
        }

        // GET: Licitacion
        [AuthorizeClaim("Licitaciones.Ver")]
        public async Task<IActionResult> Index(string codigoLicitacion, string titulo, DateTime? fechaPublicacion, DateTime? fechaCierre, int? idCategoriaLicitacion, int? idEstadoLicitacion, int page = 1, int pageSize = 10)
        {
            var licitacionesList = await _licitacionManager.GetAllLicitacionesAsync();
            var categorias = await _licitacionManager.GetAllCategoriasLicitacionAsync();
            var estados = (await _licitacionManager.GetAllEstadosLicitacionAsync())
                .Where(e => e.Enabled)
                .ToList();
            ViewBag.CategoriasLicitacion = categorias;
            ViewBag.EstadosLicitacion = estados;

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

            if (idEstadoLicitacion.HasValue)
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
        [AuditarEvento("LicitacionController - Details", "Licitacion", "Visualización de detalle de licitación", "id")]
        public async Task<IActionResult> Details(int? id)
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
            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();

            var entidad = _dbContext.EntidadesLicitantes.FirstOrDefault(e => e.IdEntidadLicitante == licitacion.IdEntidadLicitante);
            string cuit = entidad?.Cuit;
            string razon = entidad?.RazonSocial;
            string entidadLicitanteFormateada = FormatearCuitSeguro(cuit, razon);

            await _auditManager.LogLicitacionChange(
                licitacion.IdLicitacion,
                IdentityHelper.GetUserLicitARId(User),
                "Visualización Detalle",
                null, null, null
            );

            var vm = new LicitacionDetailsViewModel
            {
                IdLicitacion = licitacion.IdLicitacion,
                IdEntidadLicitante = licitacion.IdEntidadLicitante,
                CodigoLicitacion = licitacion.CodigoLicitacion,
                Titulo = licitacion.Titulo,
                Descripcion = licitacion.Descripcion,
                FechaPublicacion = licitacion.FechaPublicacion,
                FechaCierre = licitacion.FechaCierre,
                IdEstadoLicitacion = licitacion.IdEstadoLicitacion,
                IdCategoriaLicitacion = licitacion.IdCategoriaLicitacion,
                EntidadLicitanteFormateada = entidadLicitanteFormateada,
                Items = licitacion.Items.Select(x => new LicitacionDetalleModel
                {
                    IdLicitacionDetalle = x.IdLicitacionDetalle,
                    IdLicitacion = x.IdLicitacion,
                    NroItem = x.NroItem,
                    Item = x.Item,
                    Descripcion = x.Descripcion,
                    Cantidad = x.Cantidad,
                    PrecioEstimadoUnitario = x.PrecioEstimadoUnitario,
                    Eliminado = false
                }).ToList(),
                EstadoLicitacion = licitacion.EstadoLicitacion
            };

            return View(vm);
        }

        // GET: Licitacion/Create
        [AuthorizeClaim("Licitaciones.Crear")]
        [AuditarEvento("LicitacionController - Create", "Licitacion", "Inicio creación de licitación")]
        public IActionResult Create()
        {
            var entidades = _dbContext.EntidadesLicitantes
                .Where(e => e.Enabled)
                .ToList()
                .Select(e => new
                {
                    e.IdEntidadLicitante,
                    Texto = FormatearCuitSeguro(e.Cuit, e.RazonSocial)
                }).ToList();

            ViewBag.EntidadesLicitantes = entidades;
            return View();
        }

        // POST: Licitacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Crear")]
        [AuditarEvento("LicitacionController - Create", "Licitacion", "Creación de licitación")]
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
                await _auditManager.LogLicitacionChange(
                    licitacion.IdLicitacion,
                    IdentityHelper.GetUserLicitARId(User),
                    "Creación",
                    null, null, null
                );
                TempData["Mensaje"] = "Licitación Creada Exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(licitacionModel);
        }

        // GET: Licitacion/Edit/5
        [AuthorizeClaim("Licitaciones.Editar")]
        [AuditarEvento("LicitacionController - Edit", "Licitacion", "Inicio edición de licitación", "id")]
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

            var entidades = _dbContext.EntidadesLicitantes
                .Where(e => e.Enabled)
                .ToList()
                .Select(e => new
                {
                    e.IdEntidadLicitante,
                    Texto = FormatearCuitSeguro(e.Cuit, e.RazonSocial)
                }).ToList();

            ViewBag.EntidadesLicitantes = entidades;

            var lic = new LicitacionModel();
            lic.SetLicitacionData(licitacion);
            return View(lic);
        }

        // POST: Licitacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Editar")]
        [AuditarEvento("LicitacionController - Edit", "Licitacion", "Edición de licitación", "id")]
        public async Task<IActionResult> Edit(int id, LicitacionModel licitacionModel)
        {
            if (id != licitacionModel.IdLicitacion)
            {
                return View("NotFound");
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
                    return View("NotFound");
                }
                await _auditManager.LogLicitacionChange(
                    licitacion.IdLicitacion,
                    IdentityHelper.GetUserLicitARId(User),
                    "Edición",
                    null, null, null
                );
                TempData["Mensaje"] = "Licitación Modificada Exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(licitacionModel);
        }

        // GET: Licitacion/Delete/5
        [AuthorizeClaim("Licitaciones.Eliminar")]
        [AuditarEvento("LicitacionController - Delete", "Licitacion", "Inicio eliminación de licitación", "id")]
        public async Task<IActionResult> Delete(int? id)
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

            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
            return View(licitacion);
        }

        // POST: Licitacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Eliminar")]
        [AuditarEvento("LicitacionController - DeleteConfirmed", "Licitacion", "Confirmación de eliminación de licitación", "id")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _licitacionManager.DeleteLicitacionAsync(id, IdentityHelper.GetUserLicitARId(User));
            if (!result)
            {
                return View("NotFound");
            }
            await _auditManager.LogLicitacionChange(
                id,
                IdentityHelper.GetUserLicitARId(User),
                "Eliminación",
                null, null, null
            );
            TempData["Mensaje"] = "Licitación Eliminada Exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Licitacion/Publicar/5
        [AuthorizeClaim("Licitaciones.Editar")]
        [AuditarEvento("LicitacionController - Publicar", "Licitacion", "Inicio publicación de licitación", "id")]
        public async Task<IActionResult> Publicar(int? id)
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

            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
            return View(licitacion);
        }

        // POST: Licitacion/Publicar/5
        [HttpPost, ActionName("Publicar")]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("Licitaciones.Crear")]
        [AuditarEvento("LicitacionController - PublicarConfirmed", "Licitacion", "Confirmación de publicación de licitación", "IdLicitacion")]
        public async Task<IActionResult> PublicarConfirmed(LicitacionPublicarConfirmModel licitacion)
        {
            var result = await _licitacionManager.PublicarLicitacionAsync(licitacion.IdLicitacion, licitacion.FechaCierre, IdentityHelper.GetUserLicitARId(User));
            if (!result)
            {
                return View("NotFound");
            }
            await _auditManager.LogLicitacionChange(
                licitacion.IdLicitacion,
                IdentityHelper.GetUserLicitARId(User),
                "Publicación",
                "FechaCierre", null, licitacion.FechaCierre.ToString()
            );
            TempData["Mensaje"] = "Licitación Publicada Exitosamente!";

            return RedirectToAction(nameof(Index));
        }

        // GET: Licitacion/Evaluar/5
        [AuthorizeClaim("Licitaciones.Crear")]
        public async Task<IActionResult> Evaluar(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var evaluacionExistente = await _evaluacionManager.GetEvaluacionByLicitacionAsync(id.Value);
            if (evaluacionExistente != null)
            {
                await _auditManager.LogLicitacionChange(
                    id.Value,
                    IdentityHelper.GetUserLicitARId(User),
                    "Evaluar (redirige a Edit Evaluación)",
                    null, null, null
                );
                return RedirectToAction("Edit", "Evaluaciones", new { idEvaluacion = evaluacionExistente.IdEvaluacion });
            }

            bool iniciarEvaluacion = await _licitacionManager.IniciarEvaluacionLicitacionAsync(id.Value, IdentityHelper.GetUserLicitARId(User));

            if (iniciarEvaluacion)
            {
                await _auditManager.LogLicitacionChange(
                    id.Value,
                    IdentityHelper.GetUserLicitARId(User),
                    "Evaluar (redirige a Crear Evaluación)",
                    null, null, null
                );
                return RedirectToAction("Create", "Evaluaciones", new { idLicitacion = id });
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

            await _auditManager.LogLicitacionChange(
                licitacion.IdLicitacion,
                IdentityHelper.GetUserLicitARId(User),
                "Visualización Oferentes",
                null, null, null
            );

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
        [AuditarEvento("LicitacionController - History", "Licitacion", "Visualización historial de licitación", "id")]
        public async Task<IActionResult> History(int id)
        {
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(id);
            if (licitacion == null)
            {
                _logger.LogWarning("No se encontró la licitación con ID {Id}", id);
                return View("NotFound");
            }

            await _auditManager.LogLicitacionChange(
                licitacion.IdLicitacion,
                IdentityHelper.GetUserLicitARId(User),
                "Visualización Historial",
                null, null, null
            );

            var historial = await _licitacionManager.GetHistorialEstados(id);

            ViewBag.Licitacion = licitacion;
            return View(historial);
        }
    }
}
