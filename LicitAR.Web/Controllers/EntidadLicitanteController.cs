using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using LicitAR.Core.Business.Licitaciones;
using LicitAR.Core.Business.Identidad;
using LicitAR.Web.Models;
using LicitAR.Web.Helpers;
using LicitAR.Web.Helpers.Authorization;

namespace LicitAR.Web.Controllers
{
    public class EntidadLicitanteController : Controller
    {
        private readonly LicitARDbContext _context;
        private readonly IEntidadLicitanteManager _entidadLicitanteManager;
        private IMessageManager _messageManager;
        private readonly IUsuarioManager _usuarioManager;
        private readonly ILogger<EntidadLicitanteController> _logger;

        public EntidadLicitanteController(LicitARDbContext context, IEntidadLicitanteManager entidadLicitanteManager, IMessageManager message, IUsuarioManager usuarioManager, ILogger<EntidadLicitanteController> logger)
        {
            _context = context;
            _entidadLicitanteManager = entidadLicitanteManager;
            _messageManager = message;
            _usuarioManager = usuarioManager;
            _logger = logger;
        }

        // GET: EntidadLicitante
        [AuthorizeClaim("EntidadLicitante.Ver")]
        public async Task<IActionResult> Index(string cuit, string razonSocial, int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Index action called with filters: cuit={Cuit}, razonSocial={RazonSocial}, page={Page}, pageSize={PageSize}", cuit, razonSocial, page, pageSize);

            var query = _context.EntidadesLicitantes
                .Include(e => e.Provincia) // Include Provincia
                .Include(e => e.Localidad) // Include Localidad
                .AsQueryable();

            if (!string.IsNullOrEmpty(cuit))
            {
                cuit = new string(cuit.Where(char.IsDigit).ToArray()); // Remove non-numeric characters
                if (cuit.Length > 11)
                {
                    //ModelState.AddModelError("cuit", "El CUIT no puede tener más de 11 caracteres.");
                    _logger.LogWarning("CUIT filter is invalid: {Cuit}", cuit);
                }
                else
                {
                    query = query.Where(e => e.Cuit.Contains(cuit));
                    _logger.LogInformation("CUIT filter applied: {Cuit}", cuit);
                }
            }

            if (!string.IsNullOrEmpty(razonSocial))
            {
                var razonSocialLower = razonSocial.ToLower();
                query = query.Where(l => l.RazonSocial.ToLower().Contains(razonSocialLower));
                _logger.LogInformation("RazonSocial filter applied: {RazonSocial}", razonSocial);
            }

            /* if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid. Errors: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = 1;
                return View(new List<EntidadLicitante>());
            } */

            var totalItems = await query.CountAsync();
            _logger.LogInformation("Total items after filters: {TotalItems}", totalItems);

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            _logger.LogInformation("Items retrieved for current page: {ItemCount}", items.Count);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        // GET: EntidadLicitante/Details/5
        [AuthorizeClaim("EntidadLicitante.Ver")]
        public async Task<IActionResult> Details(int id)
        {
            var entidad = await _entidadLicitanteManager.GetEntidadLicitanteByIdAsync(id);
            if (entidad == null)
                return NotFound();

            var usuariosAsociados = await _entidadLicitanteManager.GetUsuariosAsociadosAsync(id);
            ViewBag.UsuariosAsociados = usuariosAsociados;

            return View(entidad);
        }

        // GET: EntidadLicitante/Create
        [AuthorizeClaim("EntidadLicitante.Crear")]
        public IActionResult Create()
        {
            var items = _context.Provincias
                    .Select(x => new SelectListItem
                    {
                        Value = x.IdProvincia.ToString(),
                        Text = x.Descripcion
                    })
                    .ToList();
            var itemsLocalidades = _context.Localidades.ToList();

            ViewBag.ComboProvincias = items;
            ViewBag.ComboLocalidades = itemsLocalidades;

            return View();
        }

        // POST: EntidadLicitante/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("EntidadLicitante.Crear")]
        public async Task<IActionResult> Create(EntidadLicitanteModel entidadLicitanteModel)
        {
            if (ModelState.IsValid)
            {
                var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                var provincia = await _entidadLicitanteManager.GetProvinciaByIdAsync(entidadLicitanteModel.IdProvincia);
                var localidad = await _entidadLicitanteManager.GetLocalidadByIdAsync(entidadLicitanteModel.IdLocalidad);

                EntidadLicitante entidadLicitante = entidadLicitanteModel.GetEntidadLicitante(audit, provincia, localidad);

                this._messageManager = await _entidadLicitanteManager.AgregarAsync(entidadLicitante, IdentityHelper.GetUserLicitARId(User));

                ViewBag.Messages = _messageManager.messages;

                if (!_messageManager.HasErrors)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(entidadLicitanteModel);
                }
            }
            return View(entidadLicitanteModel);
        }

        // GET: EntidadLicitante/Edit/5
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _entidadLicitanteManager.GetEntidadLicitanteByIdAsync(id.Value);
            if (entidadLicitante == null)
            {
                return NotFound();
            }

            // Cargar provincias y localidades para los combos
            ViewBag.ComboProvincias = _context.Provincias
                .Select(x => new SelectListItem
                {
                    Value = x.IdProvincia.ToString(),
                    Text = x.Descripcion
                })
                .ToList();

            ViewBag.ComboLocalidades = _context.Localidades
                .Select(x => new SelectListItem
                {
                    Value = x.IdLocalidad.ToString(),
                    Text = x.Descripcion
                })
                .ToList();

            EntidadLicitanteModel ent = new EntidadLicitanteModel();
            ent.SetEntidadLicitanteData(entidadLicitante);
            return View(ent);
        }

        // POST: EntidadLicitante/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> Edit(int id, EntidadLicitanteModel entidadLicitanteModel)
        {
            if (id != entidadLicitanteModel.IdEntidadLicitante)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var audit = AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User));
                    var provincia = await _entidadLicitanteManager.GetProvinciaByIdAsync(entidadLicitanteModel.IdProvincia);
                    var localidad = await _entidadLicitanteManager.GetLocalidadByIdAsync(entidadLicitanteModel.IdLocalidad);

                    var entidadLicitante = entidadLicitanteModel.GetEntidadLicitante(audit, provincia, localidad);
                    entidadLicitante.IdEntidadLicitante = entidadLicitanteModel.IdEntidadLicitante;

                    _messageManager = await _entidadLicitanteManager.ModificarAsync(entidadLicitante, id, IdentityHelper.GetUserLicitARId(User));
                }
                catch (Exception ex)
                {
                    _messageManager.ErrorMessage("Excepción al intentar modificar una entidad licitante: Ex - " + ex.ToString());
                }
                finally
                {
                    ViewBag.messages = _messageManager.messages;
                }

                if (_messageManager.HasErrors)
                    return View(entidadLicitanteModel);
                else
                    return RedirectToAction(nameof(Index));
            }
            return View(entidadLicitanteModel);
        }

        // GET: EntidadLicitante/Delete/5
        [AuthorizeClaim("EntidadLicitante.Eliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _entidadLicitanteManager.GetEntidadLicitanteByIdAsync(id.Value);
            if (entidadLicitante == null)
            {
                return NotFound();
            }

            return View(entidadLicitante);
        }

        // POST: EntidadLicitante/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeClaim("EntidadLicitante.Eliminar")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            _messageManager = await _entidadLicitanteManager.DeleteEntidadLicitanteAsync(id, IdentityHelper.GetUserLicitARId(User));

            ViewBag.messages = _messageManager.messages;

            if (_messageManager.HasErrors)
                return View(id);
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> AsociarUsuario(int idEntidadLicitante, string idUsuario)
        {
            _messageManager = await _entidadLicitanteManager.AsociarUsuarioAsync(idEntidadLicitante, idUsuario, IdentityHelper.GetUserLicitARId(User));
            ViewBag.Messages = _messageManager.messages;

            return RedirectToAction(nameof(Details), new { id = idEntidadLicitante });
        }

        [HttpPost]
        public async Task<IActionResult> DesasociarUsuario(int idEntidadLicitante, string idUsuario)
        {
            int idUser = IdentityHelper.GetUserLicitARId(User);
            await _entidadLicitanteManager.DesasociarUsuarioAsync(idEntidadLicitante, idUsuario, idUser);
            return RedirectToAction("Details", new { id = idEntidadLicitante });
        }

        [HttpGet]
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> AsociarUsuario(int idEntidadLicitante)
        {
            var entidadLicitante = await _entidadLicitanteManager.GetEntidadLicitanteByIdAsync(idEntidadLicitante);
            if (entidadLicitante == null)
            {
                return NotFound();
            }

            var usuarios = await _usuarioManager.GetAllUsersAsync();
            ViewBag.UsuariosDisponibles = usuarios.Select(u => new SelectListItem
            {
                Value = u.IdUsuario.ToString(),
                Text = $"{u.Nombre} {u.Apellido} ({u.Email})"
            }).ToList();

            ViewBag.EntidadLicitante = entidadLicitante;
            return View();
        }

        [HttpPost]
        [AuthorizeClaim("EntidadLicitante.Editar")]
        public async Task<IActionResult> AsociarUsuario(int idEntidadLicitante, List<string> selectedUsuarios)
        {
            if (selectedUsuarios == null || !selectedUsuarios.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un usuario.");
                return await AsociarUsuario(idEntidadLicitante);
            }

            foreach (var idUsuario in selectedUsuarios)
            {
                await _entidadLicitanteManager.AsociarUsuarioAsync(idEntidadLicitante, idUsuario, IdentityHelper.GetUserLicitARId(User));
            }

            return RedirectToAction(nameof(Details), new { id = idEntidadLicitante });
        }

        [HttpGet]
        public JsonResult GetLocalidadesByProvincia(int idProvincia)
        {
            var localidades = _context.Localidades
                .Where(l => l.IdProvincia == idProvincia)
                .Select(l => new { l.IdLocalidad, l.Descripcion })
                .ToList();
            return Json(localidades);
        }

        private bool EntidadLicitanteExists(int id)
        {
            return _context.EntidadesLicitantes.Any(e => e.IdEntidadLicitante == id);
        }
    }
}
