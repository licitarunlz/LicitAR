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
using LicitAR.Web.Helpers.Auditoria;

namespace LicitAR.Web.Controllers
{
    [AuthorizeClaim("Auditoria.Ver")]
    public class AuditController : Controller
    {
        private readonly IAuditManager _auditManager;
        private readonly IUsuarioManager _usuarioManager;

        public AuditController(IAuditManager auditManager, IUsuarioManager usuarioManager)
        {
            _auditManager = auditManager;
            _usuarioManager = usuarioManager;
        }

        // GET: /Audit/Trail
        [AuthorizeClaim("Auditoria.Trail")]
        [AuditarEvento("AuditController - Trazos", "Auditoria", "Visualización de trail de auditoría")]
        public async Task<IActionResult> Trail(string accion, int? usuarioId, string entidad, DateTime? desde, DateTime? hasta, string usuarioMail, string usuarioNombreCompleto, int page = 1, int pageSize = 20)
        {
            // 1. Obtener los IDs de usuario de los registros filtrados (sin paginar)
            var (allItems, _) = await _auditManager.GetAuditTrailsAsync(
                accion, usuarioId, entidad, desde, hasta, null, 1, int.MaxValue);
            var usuarioIdsFiltrados = allItems.Select(x => x.UsuarioId).Distinct().ToList();

            // 2. Obtener info de esos usuarios
            var usuariosDict = await _usuarioManager.GetUsuariosInfoByIdsAsync(usuarioIdsFiltrados);

            // 3. Filtrar por mail/nombre si corresponde
            HashSet<int>? usuarioIdsFilter = null;
            if (!string.IsNullOrWhiteSpace(usuarioMail))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => (u.Value.Email ?? "").ToLower().Contains(usuarioMail.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                usuarioIdsFilter = idsFiltrados;
            }
            if (!string.IsNullOrWhiteSpace(usuarioNombreCompleto))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => ((u.Value.Nombre + " " + u.Value.Apellido).Trim().ToLower()).Contains(usuarioNombreCompleto.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                usuarioIdsFilter = usuarioIdsFilter == null ? idsFiltrados : usuarioIdsFilter.Intersect(idsFiltrados).ToHashSet();
            }

            // 4. Obtener los registros paginados con el filtro de usuarioIds si corresponde
            var (items, total) = await _auditManager.GetAuditTrailsAsync(
                accion, usuarioId, entidad, desde, hasta, usuarioIdsFilter, page, pageSize);

            var usuarioIds = items.Select(x => x.UsuarioId).Distinct().ToList();
            var usuariosDictPage = await _usuarioManager.GetUsuariosInfoByIdsAsync(usuarioIds);

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
                UsuarioNombre = "",
                UsuarioMail = usuariosDictPage.ContainsKey(x.UsuarioId) ? usuariosDictPage[x.UsuarioId].Email : "",
                UsuarioNombreCompleto = usuariosDictPage.ContainsKey(x.UsuarioId)
                    ? (usuariosDictPage[x.UsuarioId].Nombre + " " + usuariosDictPage[x.UsuarioId].Apellido).Trim()
                    : ""
            }).ToList();

            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(vms);
        }

        // GET: /Audit/Licitacion
        [AuthorizeClaim("Auditoria.Licitacion")]
        [AuditarEvento("AuditController - Licitacion", "Auditoria", "Visualización de auditoría de licitación")]
        public async Task<IActionResult> Licitacion(string accion, int? usuarioId, int? idLicitacion, string campo, DateTime? desde, DateTime? hasta, string usuarioMail, string usuarioNombreCompleto, int page = 1, int pageSize = 20)
        {
            // 1. Obtener los IDs de usuario de los registros filtrados (sin paginar)
            var (allItems, _) = await _auditManager.GetAuditLicitacionesAsync(
                accion, usuarioId, idLicitacion, campo, desde, hasta, null, 1, int.MaxValue);
            var usuarioIdsFiltrados = allItems.Select(x => x.UsuarioId).Distinct().ToList();

            // 2. Obtener info de esos usuarios
            var usuariosDict = await _usuarioManager.GetUsuariosInfoByIdsAsync(usuarioIdsFiltrados);

            // 3. Filtrar por mail/nombre si corresponde
            HashSet<int>? usuarioIdsFilter = null;
            if (!string.IsNullOrWhiteSpace(usuarioMail))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => (u.Value.Email ?? "").ToLower().Contains(usuarioMail.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                usuarioIdsFilter = idsFiltrados;
            }
            if (!string.IsNullOrWhiteSpace(usuarioNombreCompleto))
            {
                var idsFiltrados = usuariosDict
                    .Where(u => ((u.Value.Nombre + " " + u.Value.Apellido).Trim().ToLower()).Contains(usuarioNombreCompleto.ToLower()))
                    .Select(u => u.Key)
                    .ToHashSet();
                usuarioIdsFilter = usuarioIdsFilter == null ? idsFiltrados : usuarioIdsFilter.Intersect(idsFiltrados).ToHashSet();
            }

            // 4. Obtener los registros paginados con el filtro de usuarioIds si corresponde
            var (items, total) = await _auditManager.GetAuditLicitacionesAsync(
                accion, usuarioId, idLicitacion, campo, desde, hasta, usuarioIdsFilter, page, pageSize);

            var usuarioIds = items.Select(x => x.UsuarioId).Distinct().ToList();
            var usuariosDictPage = await _usuarioManager.GetUsuariosInfoByIdsAsync(usuarioIds);

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
                UsuarioNombre = "",
                UsuarioMail = usuariosDictPage.ContainsKey(x.UsuarioId) ? usuariosDictPage[x.UsuarioId].Email : "",
                UsuarioNombreCompleto = usuariosDictPage.ContainsKey(x.UsuarioId)
                    ? (usuariosDictPage[x.UsuarioId].Nombre + " " + usuariosDictPage[x.UsuarioId].Apellido).Trim()
                    : "",
                LicitacionTitulo = x.Licitacion?.Titulo
            }).ToList();

            ViewBag.Total = total;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            return View(vms);
        }
    }
}
