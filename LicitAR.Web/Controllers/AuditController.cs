using LicitAR.Core.Business.Auditoria;
using LicitAR.Core.Data;
using LicitAR.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LicitAR.Web.Controllers
{
    public class AuditController : Controller
    {
        private readonly LicitARDbContext _dbContext;

        public AuditController(LicitARDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Audit/Trail
        public async Task<IActionResult> Trail(string accion, int? usuarioId, string entidad, DateTime? desde, DateTime? hasta, int page = 1, int pageSize = 20)
        {
            var query = _dbContext.AuditTrails.AsQueryable();

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(x => x.Accion.Contains(accion));
            if (usuarioId.HasValue)
                query = query.Where(x => x.UsuarioId == usuarioId.Value);
            if (!string.IsNullOrEmpty(entidad))
                query = query.Where(x => x.Entidad.Contains(entidad));
            if (desde.HasValue)
                query = query.Where(x => x.FechaHora >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(x => x.FechaHora <= hasta.Value);

            // Siempre ordenar por FechaHora descendente antes de paginar
            query = query.OrderByDescending(x => x.FechaHora);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vms = items.Select(x => new AuditTrailViewModel
            {
                Id = x.Id,
                UsuarioId = x.UsuarioId,
                FechaHora = x.FechaHora,
                Accion = x.Accion,
                Entidad = x.Entidad,
                EntidadId = x.EntidadId,
                Descripcion = x.Descripcion,
                IpCliente = x.IpCliente,
                UserAgent = x.UserAgent,
                UsuarioNombre = "" // Completar si tienes acceso a usuarios
            }).ToList();

            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(vms);
        }

        // GET: /Audit/Licitacion
        public async Task<IActionResult> Licitacion(string accion, int? usuarioId, int? idLicitacion, string campo, DateTime? desde, DateTime? hasta, int page = 1, int pageSize = 20)
        {
            var query = _dbContext.AuditLicitaciones
                .Include(x => x.Licitacion)
                .AsQueryable();

            if (!string.IsNullOrEmpty(accion))
                query = query.Where(x => x.Accion.Contains(accion));
            if (usuarioId.HasValue)
                query = query.Where(x => x.UsuarioId == usuarioId.Value);
            if (idLicitacion.HasValue)
                query = query.Where(x => x.IdLicitacion == idLicitacion.Value);
            if (!string.IsNullOrEmpty(campo))
                query = query.Where(x => x.CampoModificado.Contains(campo));
            if (desde.HasValue)
                query = query.Where(x => x.FechaHora >= desde.Value);
            if (hasta.HasValue)
                query = query.Where(x => x.FechaHora <= hasta.Value);

            // Siempre ordenar por FechaHora descendente antes de paginar
            query = query.OrderByDescending(x => x.FechaHora);

            var total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vms = items.Select(x => new AuditLicitacionViewModel
            {
                Id = x.Id,
                IdLicitacion = x.IdLicitacion,
                FechaHora = x.FechaHora,
                UsuarioId = x.UsuarioId,
                Accion = x.Accion,
                CampoModificado = x.CampoModificado,
                ValorAnterior = x.ValorAnterior,
                ValorNuevo = x.ValorNuevo,
                UsuarioNombre = "", // Completar si tienes acceso a usuarios
                LicitacionTitulo = x.Licitacion?.Titulo
            }).ToList();

            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(vms);
        }
    }
}
