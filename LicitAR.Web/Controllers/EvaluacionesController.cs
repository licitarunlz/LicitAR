using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Web.Models;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Core.Data.Models.Historial;
using LicitAR.Core.Business.Auditoria;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Web.Helpers.Auditoria;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
 
namespace LicitAR.Web.Controllers
{
    public class EvaluacionesController : Controller
    {
        private LicitARDbContext _context;
        private ILicitacionManager _licitacionManager;
        private IOfertaManager _ofertaManager;
        private IEvaluacionManager _evaluacionManager;
        private readonly IAuditManager _auditManager;
        private readonly IConfiguration _config;

        public EvaluacionesController(LicitARDbContext context,
                                      ILicitacionManager licitacionManager,
                                      IOfertaManager ofertaManager,
                                      IEvaluacionManager evaluacionManager,
                                      IAuditManager auditManager,
                                      IConfiguration config)
        {
            _context = context;
            _licitacionManager = licitacionManager;
            _ofertaManager = ofertaManager;
            _evaluacionManager = evaluacionManager;
            _auditManager = auditManager;
            _config = config;
        }

        [HttpGet("/Licitacion/Evaluaciones")]
        [AuditarEvento("EvaluacionesController - Tabla", "Evaluacion", "Visualización de listado de evaluaciones")]
        public async Task<IActionResult> Index(string codigoLicitacion, int? idEstadoEvaluacion, int page = 1, int pageSize = 10)
        {
            ViewBag.EstadosEvalacion = _context.EstadoEvaluacion.ToList();

            var licitaciones = await _evaluacionManager.GetAllEvaluacionesAsync();

            licitaciones = licitaciones.OrderByDescending(x => x.Licitacion.CodigoLicitacion).ToList();

            var query = licitaciones.AsQueryable();
            if (!string.IsNullOrEmpty(codigoLicitacion))
            {
                query = query.Where(l => l.Licitacion.CodigoLicitacion != null && l.Licitacion.CodigoLicitacion.Contains(codigoLicitacion, StringComparison.OrdinalIgnoreCase));
            }

            if (idEstadoEvaluacion.HasValue)
            {
                query = query.Where(l => l.IdEstadoEvaluacion == idEstadoEvaluacion.Value);
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var licitacionesList = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(licitacionesList);
        }

        // GET: Evaluaciones/Details/5
        [HttpGet("/Licitacion/Evaluaciones/{idEvaluacion:int}/Detalles")]
        [AuditarEvento("EvaluacionesController - Detalle", "Evaluacion", "Visualización de detalle de evaluación", "idEvaluacion")]
        public async Task<IActionResult> Details(int? idEvaluacion)
        {

            if (idEvaluacion == null)
                return View("NotFound");

            var evaluacion = await _evaluacionManager.GetEvaluacionByIdAsync(idEvaluacion.Value);

            if (evaluacion == null)
            {
                return View("NotFound"); // Updated
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(evaluacion.IdLicitacion);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }


            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();

            ViewBag.licitacion = licitacion;

            var ofertas = await _ofertaManager.GetAllOfertasPorLicitacionAsync(evaluacion.IdLicitacion);
            ofertas = ofertas.Where(x => x.IdEstadoOferta == 2).ToList();
            ViewBag.ofertas = ofertas;
            EvaluacionModel model = new EvaluacionModel();
            model.SetEvaluacion(evaluacion);
            var ofertasGanadoras = evaluacion.EvaluacionOfertasDetalles.Select(x => x.IdOfertaDetalle);
            var ofertasGanadorasModel = new Dictionary<int, int>();

            foreach (var oferta in ofertas)
            {
                foreach (var ofertaDetalle in oferta.Items)
                {

                    if (ofertasGanadoras.Contains(ofertaDetalle.IdOfertaDetalle))
                    {
                        ofertasGanadorasModel.Add(ofertaDetalle.IdLicitacionDetalle, ofertaDetalle.IdOfertaDetalle);
                    }
                }
            }
            model.Ofertas = ofertasGanadorasModel;

            return View(model);
        }

        // GET: Evaluaciones/Create
        [HttpGet("/Licitacion/{idLicitacion:int}/Evaluaciones/Iniciar")]
        public async Task<IActionResult> Create(int idLicitacion)
        {


            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(idLicitacion);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }
            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();

            ViewBag.licitacion = licitacion;

            var ofertas = await _ofertaManager.GetAllOfertasPorLicitacionAsync(idLicitacion);
            ofertas = ofertas.Where(x => x.IdEstadoOferta == 2).ToList();
            ViewBag.ofertas = ofertas;
            EvaluacionModel model = new EvaluacionModel();
            model.IdLicitacion = idLicitacion;
            model.FechaInicioEvaluacion = DateTime.Now;
            model.IdEvaluacion = 0;
            model.FechaFinEvaluacion = DateTime.Now;
            model.IdUsuarioEvaluador = IdentityHelper.GetUserLicitARId(User);

            var oferentes = await _ofertaManager.GetOferentesPorLicitacionAsync(idLicitacion);

            ViewBag.Oferentes = oferentes;
            //ViewData["IdLicitacion"] = new SelectList(_context.Licitaciones, "IdLicitacion", "CodigoLicitacion");
            return View(model);
        }

        // POST: Evaluaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Licitacion/{idLicitacion:int}/Evaluaciones/Iniciar"), ActionName("Create")]
        [ValidateAntiForgeryToken]
        [AuditarEvento("EvaluacionesController - Crear", "Evaluacion", "Creación de evaluación", "idLicitacion")]
        public async Task<IActionResult> Create(int idLicitacion, EvaluacionModel evaluacion)
        {
            if (ModelState.IsValid)
            {
                AuditTable table = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                Evaluacion eval = evaluacion.GetEvaluacion(table);
                eval.EvaluacionOfertasDetalles = evaluacion.GetEvaluacionOferta(table).ToList();

                await _evaluacionManager.CreateEvaluacionAsync(eval, IdentityHelper.GetUserLicitARId(User));
                await _auditManager.LogLicitacionChange(
                    idLicitacion,
                    IdentityHelper.GetUserLicitARId(User),
                    "Creación Evaluación",
                    null, null, null
                );
                TempData["Mensaje"] = "Evaluación Creada Exitosamente!";
                return RedirectToAction(nameof(Index));
            }

            ViewData["IdLicitacion"] = new SelectList(_context.Licitaciones, "IdLicitacion", "CodigoLicitacion", evaluacion.IdLicitacion);

            return View(evaluacion);
        }

        // GET: /Licitacion/{idLicitacion:int}/Evaluaciones/
        [HttpGet("/Licitacion/Evaluaciones/{idEvaluacion:int}/Editar")]
        public async Task<IActionResult> Edit(int? idEvaluacion)
        {

            if (idEvaluacion == null)
                return View("NotFound");

            var evaluacion = await _evaluacionManager.GetEvaluacionByIdAsync(idEvaluacion.Value);

            if (evaluacion == null)
            {
                return View("NotFound"); // Updated
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(evaluacion.IdLicitacion);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }


            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();

            ViewBag.licitacion = licitacion;

            var ofertas = await _ofertaManager.GetAllOfertasPorLicitacionAsync(evaluacion.IdLicitacion);
            ofertas = ofertas.Where(x => x.IdEstadoOferta == 2).ToList();
            ViewBag.ofertas = ofertas;
            EvaluacionModel model = new EvaluacionModel();
            model.SetEvaluacion(evaluacion);
            var ofertasGanadoras = evaluacion.EvaluacionOfertasDetalles.Select(x => x.IdOfertaDetalle);
            var ofertasGanadorasModel = new Dictionary<int, int>();

            foreach (var oferta in ofertas)
            {
                foreach (var ofertaDetalle in oferta.Items)
                {

                    if (ofertasGanadoras.Contains(ofertaDetalle.IdOfertaDetalle))
                    {
                        ofertasGanadorasModel.Add(ofertaDetalle.IdLicitacionDetalle, ofertaDetalle.IdOfertaDetalle);
                    }
                }
            }
            model.Ofertas = ofertasGanadorasModel;

            return View(model);
        }

        // POST: Evaluaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Licitacion/Evaluaciones/{idEvaluacion:int}/Editar")]
        [ValidateAntiForgeryToken]
        [AuditarEvento("EvaluacionesController - Editar", "Evaluacion", "Edición de evaluación", "idEvaluacion")]
        public async Task<IActionResult> Edit(int idEvaluacion, EvaluacionModel evaluacionModel)
        {


            if (idEvaluacion != evaluacionModel.IdEvaluacion) // Fix comparison to match IdLicitacion
            {
                return View("NotFound"); // Updated
            }

            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));

                var evaluacion = evaluacionModel.GetEvaluacion(audit);
                evaluacion.IdEvaluacion = evaluacionModel.IdEvaluacion;
                evaluacion.EvaluacionOfertasDetalles = evaluacionModel.GetEvaluacionOferta(audit).ToList();

                var result = await _evaluacionManager.UpdateEvaluacionAsync(evaluacion, IdentityHelper.GetUserLicitARId(User));
                if (!result)
                {
                    return View("NotFound"); // Updated
                }
                await _auditManager.LogLicitacionChange(
                    evaluacion.IdLicitacion,
                    IdentityHelper.GetUserLicitARId(User),
                    "Edición Evaluación",
                    null, null, null
                );
                TempData["Mensaje"] = "Evaluación Modificada Exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(evaluacionModel);

        }

        // GET: Evaluaciones/Resultado/5
        [HttpGet("/Licitacion/Evaluaciones/{idEvaluacion:int}/Resultado/{idEstadoResultado:int}")]
        [AuditarEvento("EvaluacionesController - Resultado Generar", "Evaluacion", "Visualización de resultado de evaluación", "idEvaluacion")]
        public async Task<IActionResult> Resultado(int? idEvaluacion, int? idEstadoResultado)
        {
            if (idEvaluacion == null)
                return View("NotFound");

            var evaluacion = await _evaluacionManager.GetEvaluacionByIdAsync(idEvaluacion.Value);

            if (evaluacion == null)
            {
                return View("NotFound"); // Updated
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(evaluacion.IdLicitacion);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }


            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();

            ViewBag.licitacion = licitacion;

            var ofertas = await _ofertaManager.GetAllOfertasPorLicitacionAsync(evaluacion.IdLicitacion);

            ViewBag.ofertas = ofertas;
            EvaluacionModel model = new EvaluacionModel();
            model.SetEvaluacion(evaluacion);
            var ofertasGanadoras = evaluacion.EvaluacionOfertasDetalles.Select(x => x.IdOfertaDetalle);
            var ofertasGanadorasModel = new Dictionary<int, int>();

            foreach (var oferta in ofertas)
            {
                foreach (var ofertaDetalle in oferta.Items)
                {

                    if (ofertasGanadoras.Contains(ofertaDetalle.IdOfertaDetalle))
                    {
                        ofertasGanadorasModel.Add(ofertaDetalle.IdLicitacionDetalle, ofertaDetalle.IdOfertaDetalle);
                    }
                }
            }

            ViewBag.IdEstadoResultado = idEstadoResultado;

            model.Ofertas = ofertasGanadorasModel;

            return View(model); 
        }

        // POST: Evaluaciones/Delete/5
        [HttpPost("/Licitacion/Evaluaciones/{idEvaluacion:int}/Resultado/{idEstadoResultado:int}")]
        [ValidateAntiForgeryToken]
        [AuditarEvento("EvaluacionesController - Resultado Enviar", "Evaluacion", "Actualización de resultado de evaluación", "idEvaluacion")]
        public async Task<IActionResult> Resultado(int idEvaluacion, int idEstadoResultado)
        {
            int idUser = IdentityHelper.GetUserLicitARId(User);
            var baseUrl = _config["App:BaseUrl"] ?? "https://https://licitarappdev.azurewebsites.net/";
            var result = await _evaluacionManager.ResultadoEvaluacionAsync(idEvaluacion, idEstadoResultado, idUser, baseUrl);

            return RedirectToAction(nameof(Index));
        }

        private bool EvaluacionExists(int id)
        {
            return _context.Evaluaciones.Any(e => e.IdEvaluacion == id);
        }
    }
}
