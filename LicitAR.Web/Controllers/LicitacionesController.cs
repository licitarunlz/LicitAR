using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using Microsoft.IdentityModel.Tokens;
using LicitAR.Core.Utils;
using LicitAR.Web.Helpers;

namespace LicitAR.Web.Controllers
{
    public class LicitacionesController : Controller
    {
        private readonly LicitacionesDbContext _context;

        public LicitacionesController(LicitacionesDbContext context)
        {
            _context = context;
        }

        // GET: Licitaciones
        public async Task<IActionResult> Index()
        {
            return View(await _context.Licitaciones.ToListAsync());
        }

        // GET: Licitaciones/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licitacion = await _context.Licitaciones
                .FirstOrDefaultAsync(m => m.IdEntidadLicitante == id);
            if (licitacion == null)
            {
                return NotFound();
            }

            return View(licitacion);
        }

        // GET: Licitaciones/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Licitaciones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdLicitacion,IdEntidadLicitante,CodigoLicitacion,Titulo,Descripcion,FechaPublicacion,FechaCierre,IdEstadoLicitacion,IdCategoriaLicitacion")] Licitacion licitacion)
        {
            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                licitacion.Audit = audit;
                _context.Add(licitacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(licitacion);
        }

        // GET: Licitaciones/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licitacion = await _context.Licitaciones.FindAsync(id);
            if (licitacion == null)
            {
                return NotFound();
            }
            return View(licitacion);
        }

        // POST: Licitaciones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdLicitacion,IdEntidadLicitante,CodigoLicitacion,Titulo,Descripcion,FechaPublicacion,FechaCierre,IdEstadoLicitacion,IdCategoriaLicitacion")] Licitacion licitacion)
        {
            if (id != licitacion.IdEntidadLicitante)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(licitacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicitacionExists(licitacion.IdEntidadLicitante))
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
            return View(licitacion);
        }

        // GET: Licitaciones/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licitacion = await _context.Licitaciones
                .FirstOrDefaultAsync(m => m.IdEntidadLicitante == id);
            if (licitacion == null)
            {
                return NotFound();
            }

            return View(licitacion);
        }

        // POST: Licitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var licitacion = await _context.Licitaciones.FindAsync(id);
            if (licitacion != null)
            {
                _context.Licitaciones.Remove(licitacion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicitacionExists(int id)
        {
            return _context.Licitaciones.Any(e => e.IdEntidadLicitante == id);
        }
    }
}
