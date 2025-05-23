using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Web.Models;
using System.Net.WebSockets;
using LicitAR.Web.Helpers.Authorization;
using LicitAR.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using LicitAR.Core.Utils;
using LicitAR.Core.Data.Models.Parametros;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace LicitAR.Web.Controllers
{
    public class OfertasController : Controller
    {
        private readonly ILicitacionManager _licitacionManager;
        private readonly IOfertaManager _ofertaManager;
        private readonly LicitARDbContext _context;

        public OfertasController(LicitARDbContext context, ILicitacionManager licitacionManager, IOfertaManager ofertaManager)
        {
            _licitacionManager = licitacionManager;
            _ofertaManager = ofertaManager;
            _context = context;
        }

        // GET: Ofertas
        [HttpGet("/Proveedor/Licitaciones")]
        public async Task<IActionResult> IndexLicitaciones(string codigoLicitacion, string titulo, int? idCategoriaLicitacion, int page = 1, int pageSize = 10)
        {
            var licitaciones = await _licitacionManager.GetAllLicitacionesAsync();
            var categorias = await _licitacionManager.GetAllCategoriasAsync();
            ViewBag.CategoriasLicitacion = categorias;
            //Con esta consulta me traigo aquellas licitaciones que no hayan cerrado,
            //      que hayan sido publicadas
            //      y que no estén dadas de baja
            licitaciones = licitaciones.Where(x =>
                                           x.FechaCierre > DateTime.Now
                                        && x.FechaPublicacion < DateTime.Now
                                        && x.Audit.FechaBaja == null)?.ToList();
            if (licitaciones == null)
                return View(licitaciones);

            var query = licitaciones.AsQueryable();

            if (!string.IsNullOrEmpty(codigoLicitacion))
            {
                query = query.Where(l => l.CodigoLicitacion.Contains(codigoLicitacion));
            }

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
            }

           
            if (idCategoriaLicitacion.HasValue)
            {
                query = query.Where(l => l.IdCategoriaLicitacion == idCategoriaLicitacion.Value);
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var licitacionesVista = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;


            return View(licitacionesVista);


        }

        [AuthorizeClaim("Licitaciones.Ver")]
        [HttpGet("/Proveedor/Licitaciones/Detalle")]
        public async Task<IActionResult> DetalleLicitaciones(int? id)
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
        // GET: Ofertas
        public async Task<IActionResult> Index(string codigoLicitacion, string titulo,   int page = 1, int pageSize = 10)
        {

            int idPersona = int.Parse(IdentityHelper.GetUserLicitARClaim(User, "IdPersona"));

            var ofertas = await _ofertaManager.GetAllOfertasPorPersonaAsync(idPersona);
            if (ofertas == null)
                return View(ofertas);

            var query = ofertas.AsQueryable();

            if (!string.IsNullOrEmpty(codigoLicitacion))
            {
                query = query.Where(l => l.Licitacion.CodigoLicitacion.Contains(codigoLicitacion));
            }

            if (!string.IsNullOrEmpty(titulo))
            {
                query = query.Where(l => l.Licitacion.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
            }

             

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var ofertasVista = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;


            return View(ofertasVista);
             
           
           
        }

        // GET: Ofertas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Ofertas
                .Include(o => o.EstadoOferta)
                .Include(o => o.Licitacion)
                .Include(o => o.Persona)
                .Include(o=> o.Items)
                .Include(o=> o.Licitacion.Items)
                .FirstOrDefaultAsync(m => m.IdOferta == id);
            if (oferta == null)
            {
                return NotFound();
            }

            return View(oferta);
        }

        // GET: Ofertas/Create

        [AuthorizeClaim("Ofertas.Crear")]
        [HttpGet("/Proveedor/Licitaciones/Postularse")]
        public async Task<IActionResult> Create(int? idlicitacion = null)
        {
            if (idlicitacion == null)
                return View("NotFound");


            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(idlicitacion.Value);
            if (licitacion == null)
            {
                return View("NotFound"); // Updated
            }
            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
            OfertaModel oferta = new OfertaModel
            {
                FechaOferta = DateTime.Now,
                IdEstadoOferta = 1,
                IdLicitacion = idlicitacion.Value,
                IdPersona = int.Parse(IdentityHelper.GetUserLicitARClaim(User, "IdPersona"))
            };
            ViewBag.licitacion = licitacion;
            return View(oferta);
        }

        // POST: Ofertas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Proveedor/Licitaciones/Postularse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OfertaModel ofertaModel)
        {

            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
               
                Oferta oferta = ofertaModel.GetOferta(audit);
                oferta.Items = ofertaModel.GetOfertaDetalles(audit);
                await _ofertaManager.CreateOfertaAsync(oferta, IdentityHelper.GetUserLicitARId(User));
                return RedirectToAction(nameof(Index));
            }
            return View(ofertaModel);


        }

        // GET: Ofertas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Ofertas.FindAsync(id);
            if (oferta == null)
            {
                return NotFound();
            }
            ViewData["IdEstadoOferta"] = new SelectList(_context.EstadosOferta, "IdEstadoOferta", "Descripcion", oferta.IdEstadoOferta);
            ViewData["IdLicitacion"] = new SelectList(_context.Licitaciones, "IdLicitacion", "CodigoLicitacion", oferta.IdLicitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Cuit", oferta.IdPersona);
            return View(oferta);
        }

        // POST: Ofertas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdOferta,IdLicitacion,IdPersona,FechaOferta,IdEstadoOferta")] Oferta oferta)
        {
            if (id != oferta.IdOferta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(oferta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfertaExists(oferta.IdOferta))
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
            ViewData["IdEstadoOferta"] = new SelectList(_context.EstadosOferta, "IdEstadoOferta", "Descripcion", oferta.IdEstadoOferta);
            ViewData["IdLicitacion"] = new SelectList(_context.Licitaciones, "IdLicitacion", "CodigoLicitacion", oferta.IdLicitacion);
            ViewData["IdPersona"] = new SelectList(_context.Personas, "IdPersona", "Cuit", oferta.IdPersona);
            return View(oferta);
        }

        // GET: Ofertas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Ofertas
                .Include(o => o.EstadoOferta)
                .Include(o => o.Licitacion)
                .Include(o => o.Persona)
                .FirstOrDefaultAsync(m => m.IdOferta == id);
            if (oferta == null)
            {
                return NotFound();
            }

            return View(oferta);
        }

        // POST: Ofertas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var oferta = await _context.Ofertas.FindAsync(id);
            if (oferta != null)
            {
                _context.Ofertas.Remove(oferta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OfertaExists(int id)
        {
            return _context.Ofertas.Any(e => e.IdOferta == id);
        }
    }
}
