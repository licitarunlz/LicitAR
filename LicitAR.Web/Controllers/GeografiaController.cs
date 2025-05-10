using LicitAR.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace LicitAR.Web.Controllers
{
    public class GeografiaController : Controller
    {
        private readonly LicitARDbContext _context;

        public GeografiaController(LicitARDbContext context)
        {
            _context = context;
        }

        [HttpGet("/Provincias/GetLocalidades")]
        public JsonResult GetLocalidades(int provinciaId)
        {
            var localidades = _context.Localidades
                                      .Where(l => l.IdProvincia == provinciaId)
                                      .Select(l => new { l.IdLocalidad, l.Descripcion })
                                      .ToList();

            return Json(localidades);
        }
        
    }
}
