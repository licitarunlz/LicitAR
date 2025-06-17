using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Helpers.Authorization;
using LicitAR.Web.Helpers.Auditoria;
using LicitAR.Web.ViewModels.Dashboard;

using LicitAR.Core.Business.Dashboard;

namespace LicitAR.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardManager _dashboardManager;

        public DashboardController(IDashboardManager dashboardManager)
        {
            _dashboardManager = dashboardManager;
        }

        // GET: /Dashboard/Index
        [AuthorizeClaim("Dashboard.Ver")]
        public IActionResult Index()
        {
            return View();
        }

        [AuthorizeClaim("DashboardAdmin.Ver")]
        public async Task<IActionResult> AdminDashboard()
        {
            var dto = await _dashboardManager.GetAdminDashboardAsync();
            var vm = new AdminDashboardViewModel
            {
                TotalLicitaciones = dto.TotalLicitaciones,
                TotalLicitacionesActivas = dto.TotalLicitacionesActivasMesActual,
                TotalAdjudicaciones = dto.TotalAdjudicacionesMesActual,
                PorcentajeAdjudicacionesVsMesAnterior = dto.PorcentajeAdjudicacionesVsMesAnterior,
                PorcentajeLicitacionesActivasVsMesAnterior = dto.PorcentajeLicitacionesActivasVsMesAnterior,
                LicitacionesActivas = dto.LicitacionesActivas,
                Adjudicaciones = dto.Adjudicaciones,
                LicitacionesPorSector = dto.LicitacionesPorSector
                    .Select(x => new AdminDashboardViewModel.LicitacionPorSector
                    {
                        Rubro = x.Rubro,
                        Cantidad = x.Cantidad,
                        Descripcion = x.Descripcion,
                        Nombre = x.Nombre
                    }).ToList(),
                UltimasLicitaciones = dto.UltimasLicitaciones
                    .Select(x => new AdminDashboardViewModel.Licitacion
                    {
                        Rubro = x.Rubro,
                        Titulo = x.Titulo,
                        MontoEstimado = x.MontoEstimado,
                        Nombre = x.Nombre
                    }).ToList()
            };
            return View(vm);
        }

        [AuthorizeClaim("DashboardOferente.Ver")]
        public async Task<IActionResult> OferenteDashboard()
        {
            int idOferente = 1;
            var claim = User.Claims.FirstOrDefault(c => c.Type == "IdPersona");
            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                idOferente = id;
            }

            var dto = await _dashboardManager.GetOferenteDashboardAsync(idOferente);
            var vm = new OferenteDashboardViewModel
            {
                NombreOferente = dto.NombreOferente,
                LicitacionesDisponibles = dto.LicitacionesDisponibles,
                LicitacionesEnCurso = dto.LicitacionesEnCurso,
                AdjudicacionesGanadas = dto.AdjudicacionesGanadas,
                TotalLicitaciones = dto.TotalLicitaciones,
                TotalLicitacionesActivas = dto.TotalLicitacionesActivasMesActual,
                TotalAdjudicaciones = dto.TotalAdjudicacionesMesActual,
                PorcentajeAdjudicacionesVsMesAnterior = dto.PorcentajeAdjudicacionesVsMesAnterior,
                PorcentajeLicitacionesActivasVsMesAnterior = dto.PorcentajeLicitacionesActivasVsMesAnterior,
                LicitacionesActivas = dto.LicitacionesActivas,
                Adjudicaciones = dto.Adjudicaciones,
                UltimasLicitaciones = dto.UltimasLicitaciones
                    .Select(x => new OferenteDashboardViewModel.Licitacion
                    {
                        Rubro = x.Rubro,
                        Titulo = x.Titulo,
                        MontoEstimado = x.MontoEstimado,
                        Nombre = x.Nombre
                    }).ToList()
            };
            return View(vm);
        }

        [AuthorizeClaim("DashboardEntidad.Ver")]
        public async Task<IActionResult> EntidadLicitanteDashboard()
        {
            int idEntidad = 21; //BORRAR!!!!!!
            var dto = await _dashboardManager.GetEntidadLicitanteDashboardAsync(idEntidad);
            var vm = new EntidadLicitanteDashboardViewModel
            {
                LicitacionesPublicadas = dto.LicitacionesPublicadas,
                LicitacionesEnEvaluacion = dto.LicitacionesEnEvaluacion,
                AdjudicacionesRealizadas = dto.AdjudicacionesRealizadas
            };
            return View(vm);
        }
    }
}
