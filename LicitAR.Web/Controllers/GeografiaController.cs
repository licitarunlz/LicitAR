using LicitAR.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Web.Controllers
{
    public class GeografiaController : Controller
    {
        private ParametrosDbContext _parametrosDbContext;

        public GeografiaController(ParametrosDbContext parametrosDbContext)
        {
            _parametrosDbContext = parametrosDbContext;

        }
        [HttpGet("/Provincias/GetLocalidades")]
        public JsonResult GetLocalidades(int provinciaId)
        {
            var localidades = _parametrosDbContext.Localidades
                                      .Where(l => l.IdProvincia == provinciaId)
                                      .Select(l => new { l.IdLocalidad, l.Descripcion })
                                      .ToList();

            return Json(localidades);
        }
        
    }
}
