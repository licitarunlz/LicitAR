using LicitAR.Core.Business.Auditoria;
using LicitAR.Core.Data;
using LicitAR.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using LicitAR.Core.Data.Models;
using LicitAR.Web.Helpers;
using LicitAR.Core.Utils;
using LicitAR.Web.Helpers.Authorization;
using Microsoft.Extensions.Logging;
using LicitAR.Core.Business.Identidad;

namespace LicitAR.Web.Controllers
{
    [AuthorizeClaim("Auditoria.Ver")]
    public class AuditController : Controller
    {
        private readonly LicitARDbContext _dbContext;
        private readonly IUsuarioManager _usuarioManager;

        public AuditController(LicitARDbContext dbContext, IUsuarioManager usuarioManager)
        {
            _dbContext = dbContext;
            _usuarioManager = usuarioManager;
        }

        // GET: /Audit/Trail
        [AuthorizeClaim("Auditoria.Trail")]
        public async Task<IActionResult> Trail(string accion, int? usuarioId, string entidad, DateTime? desde, DateTime? hasta, string usuarioMail, string usuarioNombreCompleto, int page = 1, int pageSize = 20)
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

            // Obtener los usuarioIds únicos
            var usuarioIds = items.Select(x => x.UsuarioId).Distinct().ToList();

            // Obtener los datos de usuario usando UsuarioManager
            var usuariosDict = await _usuarioManager.GetUsuariosInfoByIdsAsync(usuarioIds);

            // Filtro por mail y nombre completo (case-insensitive, contains)
            if (!string.IsNullOrWhiteSpace(usuarioMail))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => (u.Value.Email ?? "").ToLower().Contains(usuarioMail.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                items = items.Where(x => idsFiltrados.Contains(x.UsuarioId)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(usuarioNombreCompleto))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => ((u.Value.Nombre + " " + u.Value.Apellido).Trim().ToLower()).Contains(usuarioNombreCompleto.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                items = items.Where(x => idsFiltrados.Contains(x.UsuarioId)).ToList();
            }

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
                UsuarioNombre = "", // Si quieres mantenerlo
                UsuarioMail = usuariosDict.ContainsKey(x.UsuarioId) ? usuariosDict[x.UsuarioId].Email : "",
                UsuarioNombreCompleto = usuariosDict.ContainsKey(x.UsuarioId)
                    ? (usuariosDict[x.UsuarioId].Nombre + " " + usuariosDict[x.UsuarioId].Apellido).Trim()
                    : ""
            }).ToList();

            ViewBag.Total = items.Count;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(vms);
        }

        // GET: /Audit/Licitacion
        [AuthorizeClaim("Auditoria.Licitacion")]
        public async Task<IActionResult> Licitacion(string accion, int? usuarioId, int? idLicitacion, string campo, DateTime? desde, DateTime? hasta, string usuarioMail, string usuarioNombreCompleto, int page = 1, int pageSize = 20)
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

            // Obtener los usuarioIds únicos
            var usuarioIds = items.Select(x => x.UsuarioId).Distinct().ToList();

            // Obtener los datos de usuario usando UsuarioManager
            var usuariosDict = await _usuarioManager.GetUsuariosInfoByIdsAsync(usuarioIds);

            // Filtro por mail y nombre completo (case-insensitive, contains)
            if (!string.IsNullOrWhiteSpace(usuarioMail))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => (u.Value.Email ?? "").ToLower().Contains(usuarioMail.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                items = items.Where(x => idsFiltrados.Contains(x.UsuarioId)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(usuarioNombreCompleto))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => ((u.Value.Nombre + " " + u.Value.Apellido).Trim().ToLower()).Contains(usuarioNombreCompleto.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                items = items.Where(x => idsFiltrados.Contains(x.UsuarioId)).ToList();
            }

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
                UsuarioNombre = "", // Si quieres mantenerlo
                UsuarioMail = usuariosDict.ContainsKey(x.UsuarioId) ? usuariosDict[x.UsuarioId].Email : "",
                UsuarioNombreCompleto = usuariosDict.ContainsKey(x.UsuarioId)
                    ? (usuariosDict[x.UsuarioId].Nombre + " " + usuariosDict[x.UsuarioId].Apellido).Trim()
                    : "",
                LicitacionTitulo = x.Licitacion?.Titulo
            }).ToList();

            ViewBag.Total = items.Count;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(vms);
        }
    }
}
