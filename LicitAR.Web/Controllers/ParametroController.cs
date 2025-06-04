using Microsoft.AspNetCore.Mvc;
using LicitAR.Core.Business.Parametros;
using LicitAR.Core.Data.Models.Parametros;
using LicitAR.Web.Helpers.Authorization;
using LicitAR.Web.Helpers.Auditoria;

namespace LicitAR.Web.Controllers
{
    public class ParametroController : Controller
    {
        private readonly IParametrosManager _parametrosManager;

        public ParametroController(IParametrosManager parametrosManager)
        {
            _parametrosManager = parametrosManager;
        }

        // GET: Parametro/Provincias
        [AuthorizeClaim("Parametro.Ver")]
        [AuditarEvento("ParametroController - Provincias", "Parametro", "Visualización de provincias")]
        public async Task<IActionResult> Provincias(string descripcion, int page = 1, int pageSize = 10)
        {
            var provinciasList = await _parametrosManager.GetAllProvinciasAsync();
            var query = provinciasList.AsQueryable();

            if (!string.IsNullOrEmpty(descripcion))
            {
                query = query.Where(p => p.Descripcion.Contains(descripcion, StringComparison.OrdinalIgnoreCase));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var maxPagesToShow = 10;
            var startPage = Math.Max(1, page - maxPagesToShow / 2);
            var endPage = Math.Min(totalPages, startPage + maxPagesToShow - 1);

            if (endPage - startPage + 1 < maxPagesToShow) // Ensure at least 10 pages are shown
            {
                startPage = Math.Max(1, endPage - maxPagesToShow + 1);
            }

            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;

            var provincias = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(provincias);
        }

        // GET: Parametro/Localidades
        [AuthorizeClaim("Parametro.Ver")]
        [AuditarEvento("ParametroController - Localidades", "Parametro", "Visualización de localidades")]
        public async Task<IActionResult> Localidades(string descripcion, int page = 1, int pageSize = 10)
        {
            Console.WriteLine("Entering Localidades action...");

            var localidadesList = await _parametrosManager.GetLocalidadesByProvinciaAsync(0); // Fetch all localidades
            Console.WriteLine($"Fetched {localidadesList.Count} localidades from the manager.");

            var query = localidadesList.AsQueryable();

            if (!string.IsNullOrEmpty(descripcion))
            {
                Console.WriteLine($"Filtering localidades by descripcion: {descripcion}");
                query = query.Where(l => l.Descripcion.Contains(descripcion, StringComparison.OrdinalIgnoreCase));
            }

            var totalItems = query.Count();
            Console.WriteLine($"Total items after filtering: {totalItems}");

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            Console.WriteLine($"Total pages: {totalPages}");

            var maxPagesToShow = 10;
            var startPage = Math.Max(1, page - maxPagesToShow / 2);
            var endPage = Math.Min(totalPages, startPage + maxPagesToShow - 1);

            if (endPage - startPage + 1 < maxPagesToShow) // Ensure at least 10 pages are shown
            {
                startPage = Math.Max(1, endPage - maxPagesToShow + 1);
            }

            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;

            var localidades = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            Console.WriteLine($"Localidades on current page: {localidades.Count}");

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            Console.WriteLine("Exiting Localidades action...");
            return View(localidades);
        }
    }
}
