using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Web.Models;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Data.Models.Helpers;
using LicitAR.Core.Utils;

namespace LicitAR.Web.Controllers
{
    public class EvaluacionesController : Controller
    {
        private LicitARDbContext _context;
        private ILicitacionManager _licitacionManager;
        private IOfertaManager _ofertaManager;
        private IEvaluacionManager _evaluacionManager;

        public EvaluacionesController(LicitARDbContext context,
                                      ILicitacionManager licitacionManager,
                                      IOfertaManager ofertaManager,
                                      IEvaluacionManager evaluacionManager)
        {
            _context = context;
            _licitacionManager = licitacionManager;
            _ofertaManager = ofertaManager;
            _evaluacionManager = evaluacionManager;
        }

        // GET: Evaluaciones
        public async Task<IActionResult> Index()
        {
            var licitARDbContext = _context.Evaluaciones.Include(e => e.Licitacion);
            return View(await licitARDbContext.ToListAsync());
        }

        // GET: Evaluaciones/Details/5
        [HttpGet("/Licitacion/Evaluaciones/{idEvaluacion:int}/Detalles")]
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

            ViewBag.ofertas = ofertas;
            EvaluacionModel model = new EvaluacionModel();
            model.IdLicitacion = idLicitacion;
            model.FechaInicioEvaluacion = DateTime.Now;
            model.IdEvaluacion = 0;
            model.FechaFinEvaluacion = DateTime.Now;
            model.IdUsuarioEvaluador = IdentityHelper.GetUserLicitARId(User);

            //ViewData["IdLicitacion"] = new SelectList(_context.Licitaciones, "IdLicitacion", "CodigoLicitacion");
            return View(model);
        }

        // POST: Evaluaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Licitacion/{idLicitacion:int}/Evaluaciones/Iniciar"), ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idLicitacion, EvaluacionModel evaluacion)
        {
            if (ModelState.IsValid)
            {
                AuditTable table = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                Evaluacion eval = evaluacion.GetEvaluacion(table);
                eval.EvaluacionOfertasDetalles = evaluacion.GetEvaluacionOferta(table).ToList();

                await _evaluacionManager.CreateEvaluacionAsync(eval, IdentityHelper.GetUserLicitARId(User));
                return RedirectToAction(nameof(Index));
                /*_context.Add(evaluacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));*/
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEvaluacion,IdLicitacion,IdUsuarioEvaluador,FechaInicioEvaluacion,FechaFinEvaluacion")] Evaluacion evaluacion)
        {
            if (id != evaluacion.IdEvaluacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evaluacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EvaluacionExists(evaluacion.IdEvaluacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdLicitacion"] = new SelectList(_context.Licitaciones, "IdLicitacion", "CodigoLicitacion", evaluacion.IdLicitacion);
            return View(evaluacion);
        }

        // GET: Evaluaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evaluacion = await _context.Evaluaciones
                .Include(e => e.Licitacion)
                .FirstOrDefaultAsync(m => m.IdEvaluacion == id);
            if (evaluacion == null)
            {
                return NotFound();
            }

            return View(evaluacion);
        }

        // POST: Evaluaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evaluacion = await _context.Evaluaciones.FindAsync(id);
            if (evaluacion != null)
            {
                _context.Evaluaciones.Remove(evaluacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EvaluacionExists(int id)
        {
            return _context.Evaluaciones.Any(e => e.IdEvaluacion == id);
        }
    }
}
