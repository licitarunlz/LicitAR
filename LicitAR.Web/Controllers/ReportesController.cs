using LicitAR.Core.Business.Reportes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace LicitAR.Web.Controllers
{
    public class ReportesController : Controller
    {
        private IReportesManager _reportesManager;

        public ReportesController(IReportesManager reportesManager)
        {
            _reportesManager = reportesManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Exportar(string tipoEntidad)
        {
            if (!string.IsNullOrEmpty(tipoEntidad))
            {
                byte[] stream = null;
                switch (tipoEntidad)
                {
                    case "Licitaciones":
                        stream = await _reportesManager.ExportarExcelLicitaciones();
                        break;
                    case "Ofertas":

                        stream = await _reportesManager.ExportarExcelOfertas();
                        break;
                    case "Oferentes":

                        stream = await _reportesManager.ExportarExcelOferentes();
                        break; 
                    case "EntidadesLicitantes":

                        stream = await _reportesManager.ExportarExcelEntidadesLicitantes();
                        break;
                    default:
                        return View("Index");
                        
                } 
                var nombreArchivo = $"{tipoEntidad}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    nombreArchivo);

            }
            else 
                return View("Index");

        }
    }
}

