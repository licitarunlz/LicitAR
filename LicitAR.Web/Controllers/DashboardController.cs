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
                LicitacionesActivas = dto.LicitacionesActivas,
                Adjudicaciones = dto.Adjudicaciones
            };
            return View(vm);
        }

        [AuthorizeClaim("DashboardOferente.Ver")]
        public async Task<IActionResult> OferenteDashboard()
        {
            int idOferente = 1; /* BORRAR!!!! obtener id del usuario logueado */
            var dto = await _dashboardManager.GetOferenteDashboardAsync(idOferente);
            var vm = new OferenteDashboardViewModel
            {
                LicitacionesDisponibles = dto.LicitacionesDisponibles,
                LicitacionesEnCurso = dto.LicitacionesEnCurso,
                AdjudicacionesGanadas = dto.AdjudicacionesGanadas
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
