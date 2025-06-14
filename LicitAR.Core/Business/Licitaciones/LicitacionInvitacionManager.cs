using LicitAR.Core.Data;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Utils;
using Microsoft.EntityFrameworkCore;
using LicitAR.Core.Services;

namespace LicitAR.Core.Business.Licitaciones
{
    public interface ILicitacionInvitacionManager
    {
        Task<List<LicitacionInvitacion>> GetAllAsync();
        Task<List<LicitacionInvitacion>> GetByLicitacionAsync(int idLicitacion);
        Task AddInvitacionesAsync(int idLicitacion, List<int> idPersonas, int idUsuario, string baseUrl);
        Task RemoveInvitacionAsync(int idLicitacion, int idPersona);
        Task<List<LicitacionInvitacion>> GetInvitacionesByPersonaAsync(int idPersona);
    }

    public class LicitacionInvitacionManager : ILicitacionInvitacionManager
    {
        private readonly LicitARDbContext _dbContext;
        private readonly ILicitacionNotificationService _notificationService;

        public LicitacionInvitacionManager(LicitARDbContext dbContext, ILicitacionNotificationService notificationService)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
        }

        public async Task<List<LicitacionInvitacion>> GetAllAsync()
        {
            return await _dbContext.LicitacionInvitacion
                .Include(x => x.Licitacion)
                .Include(x => x.Persona)
                .ToListAsync();
        }

        public async Task<List<LicitacionInvitacion>> GetByLicitacionAsync(int idLicitacion)
        {
            return await _dbContext.LicitacionInvitacion
                .Include(x => x.Licitacion)
                .Include(x => x.Persona)
                .Where(x => x.IdLicitacion == idLicitacion)
                .ToListAsync();
        }

        public async Task<List<LicitacionInvitacion>> GetInvitacionesByPersonaAsync(int idPersona)
        {
            return await _dbContext.LicitacionInvitacion
                .Include(x => x.Licitacion)
                .Include(x => x.Persona)
                .Where(x => x.IdPersona == idPersona)
                .ToListAsync();
        }

        public async Task AddInvitacionesAsync(int idLicitacion, List<int> idPersonas, int idUsuario, string baseUrl)
        {
            var existentes = await _dbContext.LicitacionInvitacion
                .Where(x => x.IdLicitacion == idLicitacion)
                .Select(x => x.IdPersona)
                .ToListAsync();

            var nuevas = idPersonas.Except(existentes).ToList();

            foreach (var idPersona in nuevas)
            {
                _dbContext.LicitacionInvitacion.Add(new LicitacionInvitacion
                {
                    IdLicitacion = idLicitacion,
                    IdPersona = idPersona,
                    FechaInvitacion = DateTime.Now,
                    IdUsuario = idUsuario,
                    Audit = AuditHelper.GetCreationData(idUsuario)
                });

                // Obtener datos de la persona y licitación para el email
                var persona = await _dbContext.Personas.FindAsync(idPersona);
                var licitacion = await _dbContext.Licitaciones.FindAsync(idLicitacion);

                if (persona != null && licitacion != null && !string.IsNullOrEmpty(persona.Email))
                {
                    var model = new
                    {
                        NombrePersona = persona.RazonSocial,
                        LicitacionNombre = licitacion.Titulo,
                        LicitacionId = licitacion.IdLicitacion,
                        FechaInvitacion = DateTime.Now,
                        BaseUrl = baseUrl
                    };
                    await _notificationService.NotificarInvitacionLicitacionAsync(persona.Email, model);
                }
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveInvitacionAsync(int idLicitacion, int idPersona)
        {
            var invitacion = await _dbContext.LicitacionInvitacion
                .FirstOrDefaultAsync(x => x.IdLicitacion == idLicitacion && x.IdPersona == idPersona);
            if (invitacion != null)
            {
                _dbContext.LicitacionInvitacion.Remove(invitacion);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
