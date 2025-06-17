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
using LicitAR.Core.Business.Documentacion;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;

namespace LicitAR.Web.Controllers
{
    public class LicitacionDocumentacionController : Controller
    {
        private readonly ILicitacionManager _licitacionManager;
        private readonly ILicitacionDocumentacionManager _licitacionDocumentacionManager;
        private readonly LicitARDbContext _context;

        public LicitacionDocumentacionController(
            ILicitacionManager licitacionManager,
            ILicitacionDocumentacionManager licitacionDocumentacionManager,
            LicitARDbContext context)
        {
            _licitacionManager = licitacionManager;
            _licitacionDocumentacionManager = licitacionDocumentacionManager;
            _context = context;
        }

        // GET: LicitacionDocumentacion
        [HttpGet("/Licitaciones/{idLicitacion:int}/Documentacion")]
        public async Task<IActionResult> Index(int idLicitacion)
        {
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(idLicitacion);
            if (licitacion == null)
                return NotFound();

            ViewBag.licitacion = licitacion;

            var listaDocumentacion = await _licitacionDocumentacionManager.GetAllDocumentacionByIdLicitacionAsync(idLicitacion);
            var licitacionChecklistItems = await _licitacionDocumentacionManager.GetAllChecklistItemsByIdLicitacionAsync(idLicitacion);

            ViewBag.checklistItems = licitacionChecklistItems;
            return View(listaDocumentacion.ToList());
        }

       
        // GET: LicitacionDocumentacion/Adjuntar
        [HttpGet("/Licitaciones/{idLicitacion:int}/Documentacion/Adjuntar")]
        public async Task<IActionResult> Create(int idLicitacion)
        {
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(idLicitacion);
            if (licitacion == null)
                return NotFound();

            
            ViewBag.licitacion = licitacion; 
            return View();
        }

        // POST: LicitacionDocumentacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Licitaciones/{idLicitacion:int}/Documentacion/Adjuntar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int idLicitacion, LicitacionDocumentacionModel model)
        {
            if (ModelState.IsValid)
            {
                int idUser = IdentityHelper.GetUserLicitARId(User);

                LicitacionDocumentacion licitacionDocumentacion = model.GetLicitacionDocumentacion(AuditHelper.GetCreationData(idUser));

                await _licitacionDocumentacionManager.AddLicitacionDocumentacionAsync(model.IdLicitacion, licitacionDocumentacion, model.archivo, idUser);

                return RedirectToAction(nameof(Index), new { idLicitacion = idLicitacion });
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(model.IdLicitacion);
            ViewBag.licitacion = licitacion;
            return View(model);
        }


        // GET: LicitacionDocumentacion/Adjuntar
        [HttpGet("/Licitaciones/{idLicitacion:int}/Documentacion/Checklist")]
        public async Task<IActionResult> CreateChecklist(int idLicitacion)
        {
            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(idLicitacion);
            if (licitacion == null)
                return NotFound();

            ViewBag.licitacion = licitacion;

            return View();
        }

        // POST: LicitacionDocumentacion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("/Licitaciones/{idLicitacion:int}/Documentacion/Checklist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateChecklist(int idLicitacion, LicitacionChecklistItemModel model)
        {
            if (ModelState.IsValid)
            {
                int idUser = IdentityHelper.GetUserLicitARId(User);

                LicitacionChecklistItem licitacionChecklistItem = model.GetLicitacionChecklistItem(AuditHelper.GetCreationData(idUser));

                await _licitacionDocumentacionManager.AddLicitacionChecklistItemAsync(model.IdLicitacion, licitacionChecklistItem, idUser);

                return RedirectToAction(nameof(Index), new { idLicitacion = idLicitacion });
            }

            var licitacion = await _licitacionManager.GetLicitacionByIdAsync(model.IdLicitacion);
            ViewBag.licitacion = licitacion;
            return View(model);
        }


        // POST: LicitacionDocumentacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, int idLicitacion)
        {
            var idUsuario = IdentityHelper.GetUserLicitARId(User);
             await _licitacionDocumentacionManager.RemoveLicitacionDocumentacionAsync(id, idUsuario);
           
             
            return RedirectToAction(nameof(Index), new { idLicitacion = idLicitacion });
        }

        // POST: LicitacionDocumentacion/Delete/5
        [HttpPost, ActionName("DeleteChecklist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteChecklistConfirmed(int idLicitacionChecklistItem, int idLicitacion)
        {
            var idUsuario = IdentityHelper.GetUserLicitARId(User);
            await _licitacionDocumentacionManager.RemoveLicitacionChecklistItemAsync(idLicitacionChecklistItem, idUsuario);


            return RedirectToAction(nameof(Index), new { idLicitacion = idLicitacion });
        }

    }
}
