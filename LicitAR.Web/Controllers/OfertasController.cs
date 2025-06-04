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
using Humanizer;
using LicitAR.Web.Helpers.Auditoria;

namespace LicitAR.Web.Controllers
{
    public class OfertasController : Controller
    {
        private readonly ILicitacionManager _licitacionManager;
        private readonly IOfertaManager _ofertaManager;
        private readonly ILicitacionInvitacionManager _licitacionInvitacionManager;
        private readonly LicitARDbContext _context;

        public OfertasController(LicitARDbContext context, 
                                 ILicitacionManager licitacionManager, 
                                 IOfertaManager ofertaManager,
                                 ILicitacionInvitacionManager licitacionInvitacionManager)
        {
            _licitacionManager = licitacionManager;
            _ofertaManager = ofertaManager;
            _licitacionInvitacionManager = licitacionInvitacionManager;
            _context = context;
        }

        // GET: Ofertas
        [HttpGet("/Proveedor/Licitaciones")]
        [AuditarEvento("OfertasController - Tabla", "Oferta", "Visualización de licitaciones para proveedor")]
        public async Task<IActionResult> IndexLicitaciones(string codigoLicitacion, string titulo, int? idCategoriaLicitacion, int page = 1, int pageSize = 10)
        {
            var licitaciones = await _licitacionManager.GetAllLicitacionesAsync();
            var categorias = await _licitacionManager.GetAllCategoriasAsync();
            ViewBag.CategoriasLicitacion = categorias;
            //Con esta consulta me traigo aquellas licitaciones que no hayan cerrado,
            //      que hayan sido publicadas
            //      y que no estén dadas de baja
            licitaciones = licitaciones.Where(x =>
                                           x.IdEstadoLicitacion == 3
                                        && x.IdCategoriaLicitacion == 1
                                        && x.FechaCierre > DateTime.UtcNow
                                        && x.FechaPublicacion < DateTime.UtcNow
                                        && x.Audit.FechaBaja == null)?.ToList();

            if (licitaciones == null || !licitaciones.Any())
                return View(licitaciones);

            var invitacionLicitaciones = await _licitacionInvitacionManager.GetInvitacionesByPersonaAsync(int.Parse(IdentityHelper.GetUserLicitARClaim(User, "IdPersona").ToString()));

            if (invitacionLicitaciones != null)
            {
                foreach(var invitacion in invitacionLicitaciones)
                {
                    var licit = invitacion.Licitacion;
                    if (licit.FechaPublicacion < DateTime.UtcNow 
                        && licit.FechaCierre > DateTime.UtcNow 
                        && licit.Audit.FechaBaja == null)
                    {
                        if (!licitaciones.Any(x=> x.IdLicitacion == licit.IdLicitacion))
                            licitaciones.Add(licit);
                    }
                }
            }

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
        [AuditarEvento("OfertasController - Detalle", "Oferta", "Visualización de detalle de licitación para proveedor", "id")]
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
        [AuditarEvento("OfertasController - Tabla", "Oferta", "Visualización de ofertas del proveedor")]
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
        [AuditarEvento("OfertasController - Detalle", "Oferta", "Visualización de detalle de oferta", "id")]
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
                .Include(o=> o.Licitacion.EntidadLicitante)
                .Include(o=> o.Licitacion.EstadoLicitacion)
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

            var ofertas = await _ofertaManager.GetAllOfertasExistentesPorPersonaYLicitacionAsync(int.Parse(IdentityHelper.GetUserLicitARClaim(User, "IdPersona").ToString()), idlicitacion.Value);

            if (ofertas.Count > 0)
            {
                return RedirectToAction("Edit", new { idOferta = ofertas.FirstOrDefault().IdOferta });
            }

            licitacion.Items = licitacion.Items.Where(x => x.Audit.FechaBaja == null).ToList();
            OfertaModel oferta = new OfertaModel
            {
                FechaOferta = DateTime.UtcNow,
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
        [AuditarEvento("OfertasController - Crear", "Oferta", "Creación de oferta")]
        public async Task<IActionResult> Create(OfertaModel ofertaModel)
        {

            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
               
                Oferta oferta = ofertaModel.GetOferta(audit);
                oferta.Items = ofertaModel.GetOfertaDetalles(audit);
                await _ofertaManager.CreateOfertaAsync(oferta, IdentityHelper.GetUserLicitARId(User));

                TempData["Mensaje"] = "Oferta Creada Exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(ofertaModel);


        }

        // GET: Ofertas/Edit/5
        public async Task<IActionResult> Edit(int? idOferta)
        {
            if (idOferta == null)
            {
                return NotFound();
            }


            var oferta = await _context.Ofertas
                .Include(o => o.EstadoOferta)
                .Include(o => o.Licitacion)
                .Include(o => o.Persona)
                .Include(o => o.Items)
                .Include(o => o.Licitacion.Items)
                .Include(o => o.Licitacion.EntidadLicitante)
                .Include(o => o.Licitacion.EstadoLicitacion)
                .FirstOrDefaultAsync(m => m.IdOferta == idOferta);
            if (oferta == null)
            {
                return NotFound();
            }

            return View(oferta);
        }

        // POST: Ofertas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuditarEvento("OfertasController - Editar", "Oferta", "Edición de oferta", "id")]
        public async Task<IActionResult> Edit(int id,  OfertaModel ofertaModel)
        {
            Oferta oferta = null;
            if (ModelState.IsValid)
            {
                
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));

                oferta = ofertaModel.GetOferta(audit);
                oferta.Items = ofertaModel.GetOfertaDetalles(audit);
                await _ofertaManager.UpdateOfertaAsync(oferta, IdentityHelper.GetUserLicitARId(User));
                TempData["Mensaje"] = "Oferta Editada Exitosamente!";
                return RedirectToAction(nameof(Index));
            }
            return View(oferta);
  
        }

        // GET: Ofertas/Delete/5
        public async Task<IActionResult> Cancelar(int? id)
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
                .Include(o => o.Licitacion.Items)
                .Include(o => o.Licitacion.EntidadLicitante)
                .Include(o => o.Licitacion.EstadoLicitacion)
                .FirstOrDefaultAsync(m => m.IdOferta == id);
            if (oferta == null)
            {
                return NotFound();
            }

            return View("Delete",oferta);
        }

        // POST: Ofertas/Delete/5
        [HttpPost, ActionName("Cancelar")]
        [ValidateAntiForgeryToken]
        [AuditarEvento("OfertasController - Cancelar", "Oferta", "Cancelación de oferta", "id")]
        public async Task<IActionResult> CancelarConfirmed(int id)
        {
            var result = await _ofertaManager.CancelarOfertaAsync(id, IdentityHelper.GetUserLicitARId(User));

            TempData["Mensaje"] = "Oferta Cancelada Exitosamente!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Ofertas/Delete/5
        public async Task<IActionResult> Publicar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var oferta = await _context.Ofertas
                .Include(o => o.EstadoOferta)
                .Include(o => o.Licitacion)
                .Include(o => o.Persona)
                .Include(o => o.Items)
                .Include(o => o.Licitacion.Items)
                .Include(o => o.Licitacion.EntidadLicitante)
                .Include(o => o.Licitacion.EstadoLicitacion)
                .FirstOrDefaultAsync(m => m.IdOferta == id);
            if (oferta == null)
            {
                return NotFound();
            }

            return View(oferta);
        }

        // POST: Ofertas/Delete/5
        [HttpPost, ActionName("Publicar")]
        [ValidateAntiForgeryToken]
        [AuditarEvento("OfertasController - Publicar", "Oferta", "Publicación de oferta", "idOferta")]
        public async Task<IActionResult> PublicarConfirmed(int idOferta)
        {
            var result = await _ofertaManager.PublicarOfertaAsync(idOferta, IdentityHelper.GetUserLicitARId(User));

            TempData["Mensaje"] = "Oferta Publicada Exitosamente!";
            return RedirectToAction(nameof(Index));
        }

    }
}
