using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public EntidadLicitanteController(LicitARDbContext context, IEntidadLicitanteManager entidadLicitanteManager, IMessageManager message, IUsuarioManager usuarioManager)
        {
            _context = context;
            _entidadLicitanteManager = entidadLicitanteManager;
            _messageManager = message;
            _usuarioManager = usuarioManager;
        }

        // GET: EntidadLicitante
        [AuthorizeClaim("EntidadLicitante.Ver")]
        public IActionResult Index(string cuit, string razonSocial, int page = 1, int pageSize = 10)
        {
            var query = _context.EntidadesLicitantes.AsQueryable();

            if (!string.IsNullOrEmpty(cuit))
            {
                cuit = new string(cuit.Where(char.IsDigit).ToArray()); // Remove non-numeric characters
                if (cuit.Length <= 11)
                {
                    query = query.Where(e => e.Cuit.Contains(cuit));
                }
            }

            if (!string.IsNullOrEmpty(razonSocial))
            {
                query = query.Where(e => EF.Functions.Like(e.RazonSocial.ToLower(), $"%{razonSocial.ToLower()}%"));
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(items);
        }

        // GET: EntidadLicitante/Details/5
        [AuthorizeClaim("EntidadLicitante.Ver")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _context.EntidadesLicitantes
                .FirstOrDefaultAsync(m => m.IdEntidadLicitante == id);
            if (entidadLicitante == null)
            {
                return NotFound();
            }

            return View(entidadLicitante);
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
                EntidadLicitante entidadLicitante = entidadLicitanteModel.GetEntidadLicitante(audit);

                this._messageManager = await _entidadLicitanteManager.AgregarAsync(entidadLicitante, IdentityHelper.GetUserLicitARId(User));

                ViewBag.Messages = _messageManager.messages;

                if (!_messageManager.HasErrors)
                {
                    return RedirectToAction(nameof(Index));

                }else
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
        public async Task<IActionResult> Edit(int id, EntidadLicitanteModel entidadLicitante)
        {
            if (id != entidadLicitante.IdEntidadLicitante)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entidad = entidadLicitante.GetEntidadLicitante(AuditHelper.GetCreationData(IdentityHelper.GetUserLicitARId(User)));
                    entidad.IdEntidadLicitante = entidadLicitante.IdEntidadLicitante;

                    _messageManager = await _entidadLicitanteManager.ModificarAsync(entidad, id, IdentityHelper.GetUserLicitARId(User));
                }
                catch (Exception ex)
                {
                    _messageManager.ErrorMessage("Excepcion al intentar modificar una licitaciòn: Ex - " + ex.ToString());
                }
                finally
                {
                    ViewBag.messages = _messageManager.messages;
                }

                if (_messageManager.HasErrors)
                    return View(entidadLicitante);
                else 
                    return RedirectToAction(nameof(Index));
            }
            return View(entidadLicitante);
        }

        // GET: EntidadLicitante/Delete/5
        [AuthorizeClaim("EntidadLicitante.Eliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entidadLicitante = await _context.EntidadesLicitantes
                .FirstOrDefaultAsync(m => m.IdEntidadLicitante == id);
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

            _messageManager = await _entidadLicitanteManager.BajaLogicaAsync(id, IdentityHelper.GetUserLicitARId(User));

            ViewBag.messages = _messageManager.messages;

            if (_messageManager.HasErrors)
                return View(id);
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AuthorizeClaim("EntidadLicitante.AsociarUsuario")]
        public async Task<IActionResult> AsociarUsuario(int idEntidadLicitante, string idUsuario)
        {
            _messageManager = await _entidadLicitanteManager.AsociarUsuarioAsync(idEntidadLicitante, idUsuario, IdentityHelper.GetUserLicitARId(User));
            ViewBag.Messages = _messageManager.messages;

            return RedirectToAction(nameof(Details), new { id = idEntidadLicitante });
        }

        [HttpPost]
        [AuthorizeClaim("EntidadLicitante.DesasociarUsuario")]
        public async Task<IActionResult> DesasociarUsuario(int idEntidadLicitante, string idUsuario)
        {
            _messageManager = await _entidadLicitanteManager.DesasociarUsuarioAsync(idEntidadLicitante, idUsuario, IdentityHelper.GetUserLicitARId(User));
            ViewBag.Messages = _messageManager.messages;

            return RedirectToAction(nameof(Details), new { id = idEntidadLicitante });
        }

        [HttpGet]
        [AuthorizeClaim("EntidadLicitante.AsociarUsuario")]
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
        [AuthorizeClaim("EntidadLicitante.AsociarUsuario")]
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

        private bool EntidadLicitanteExists(int id)
        {
            return _context.EntidadesLicitantes.Any(e => e.IdEntidadLicitante == id);
        }
    }
}
