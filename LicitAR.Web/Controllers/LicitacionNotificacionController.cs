using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Services;
using LicitAR.Web.Helpers;

namespace LicitAR.Web.Controllers
{
    public class LicitacionNotificacionController : Controller
    {
        private readonly ILicitacionNotificationService _notificacionService;

        public LicitacionNotificacionController(ILicitacionNotificationService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string titulo, string detalle, bool? read, bool? important, int page = 1, int pageSize = 10)
        {
            int idOferente = 1;
            var claim = User.Claims.FirstOrDefault(c => c.Type == "IdPersona");
            if (claim != null && int.TryParse(claim.Value, out int idPersona))
            {
                idOferente = idPersona;
            }
            var notificacionesQuery = (await _notificacionService.GetNotificacionesPorUsuarioAsync(idOferente, int.MaxValue)).AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
                notificacionesQuery = notificacionesQuery.Where(n => n.Titulo != null && n.Titulo.Contains(titulo, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(detalle))
                notificacionesQuery = notificacionesQuery.Where(n => n.Detalle != null && n.Detalle.Contains(detalle, StringComparison.OrdinalIgnoreCase));
            if (read.HasValue)
                notificacionesQuery = notificacionesQuery.Where(n => n.Read == read.Value);
            if (important.HasValue)
                notificacionesQuery = notificacionesQuery.Where(n => n.Important == important.Value);

            var totalItems = notificacionesQuery.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var notificaciones = notificacionesQuery
                .OrderByDescending(n => n.Audit.FechaAlta)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Filtros = new { titulo, detalle, read, important };

            return View(notificaciones);
        }

        [HttpGet]
        public async Task<IActionResult> GetNotificaciones()
        {
            int idOferente = 1;
            var claim = User.Claims.FirstOrDefault(c => c.Type == "IdPersona");
            if (claim != null && int.TryParse(claim.Value, out int idPersona))
            {
                idOferente = idPersona;
            }
            Console.WriteLine($"[GetNotificaciones] idPersona: {idOferente}"); // <-- Log en consola .NET
            var notificaciones = await _notificacionService.GetNotificacionesPorUsuarioAsync(idOferente, 10);
            return Json(notificaciones);
        }

        [HttpPost]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            int idOferente = 1;
            var claim = User.Claims.FirstOrDefault(c => c.Type == "IdPersona");
            if (claim != null && int.TryParse(claim.Value, out int idPersona))
            {
                idOferente = idPersona;
            }
            await _notificacionService.MarcarComoLeidaAsync(id, idOferente);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> CantidadNoLeidas()
        {
            int idOferente = 1;
            var claim = User.Claims.FirstOrDefault(c => c.Type == "IdPersona");
            if (claim != null && int.TryParse(claim.Value, out int idPersona))
            {
                idOferente = idPersona;
            }
            int count = await _notificacionService.GetCantidadNoLeidasAsync(idOferente);
            return Json(new { count });
        }
    }
}
