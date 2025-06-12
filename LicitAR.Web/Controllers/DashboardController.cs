using Microsoft.AspNetCore.Mvc;
using LicitAR.Web.Helpers.Authorization;
using LicitAR.Web.Helpers.Auditoria;
using LicitAR.Web.ViewModels.Dashboard;

namespace LicitAR.Web.Controllers
{
    public class DashboardController : Controller
    {
        // GET: /Dashboard/Index
        [AuthorizeClaim("Dashboard.Ver")]
        public IActionResult Index()
        {
            // Aquí puedes agregar lógica para obtener datos dinámicos para la vista
            return View();
        }

        [AuthorizeClaim("DashboardAdmin.Ver")]
        public IActionResult AdminDashboard()
        {
            var vm = new AdminDashboardViewModel
            {
                // Asignar datos de ejemplo
                TotalLicitaciones = 128,
                LicitacionesActivas = 42,
                Adjudicaciones = 17,
                // ...otros datos...
            };
            return View(vm);
        }

        [AuthorizeClaim("DashboardOferente.Ver")]
        public IActionResult OferenteDashboard()
        {
            var vm = new OferenteDashboardViewModel
            {
                // Asignar datos de ejemplo
                LicitacionesDisponibles = 15,
                LicitacionesEnCurso = 3,
                AdjudicacionesGanadas = 2,
                // ...otros datos...
            };
            return View(vm);
        }

        [AuthorizeClaim("DashboardEntidad.Ver")]
        public IActionResult EntidadLicitanteDashboard()
        {
            var vm = new EntidadLicitanteDashboardViewModel
            {
                // Asignar datos de ejemplo
                LicitacionesPublicadas = 8,
                LicitacionesEnEvaluacion = 2,
                AdjudicacionesRealizadas = 1,
                // ...otros datos...
            };
            return View(vm);
        }
    }
}
