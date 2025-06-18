using LicitAR.Core.Services;
using LicitAR.Core.Data.Models;
using LicitAR.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace LicitAR.Web.Services
{
    public class LicitacionNotificationService : ILicitacionNotificationService
    {
        private readonly EmailSenderService _emailSenderService;
        private readonly LicitARDbContext _dbContext;

        public LicitacionNotificationService(EmailSenderService emailSenderService, LicitARDbContext dbContext)
        {
            _emailSenderService = emailSenderService;
            _dbContext = dbContext;
        }

        public async Task NotificarInvitacionLicitacionAsync(string email, object model)
        {
            await _emailSenderService.SendInvitacionLicitacionAsync(email, model);
        }

        public async Task NotificarLicitacionImpugnadaAsync(string email, object model)
        {
            await _emailSenderService.SendLicitacionImpugnadaAsync(email, model);
        }

        public async Task NotificarLicitacionAdjudicadaAsync(string email, object model)
        {
            await _emailSenderService.SendLicitacionAdjudicadaAsync(email, model);
        }

        // Notificaciones en base de datos
        public async Task CrearNotificacionAsync(LicitacionNotificacion notificacion)
        {
            _dbContext.LicitacionNotificaciones.Add(notificacion);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<LicitacionNotificacion>> GetNotificacionesPorUsuarioAsync(int idPersona, int max = 10)
        {
            // Forzar máximo de 10 notificaciones
            int take = Math.Min(max, 10);
            return await _dbContext.LicitacionNotificaciones
                .Where(n => n.IdPersona == idPersona)
                .OrderByDescending(n => n.Audit.FechaAlta)
                .Take(take)
                .ToListAsync();
        }

        public async Task MarcarComoLeidaAsync(int idNotificacion, int idPersona)
        {
            var notif = await _dbContext.LicitacionNotificaciones
                .FirstOrDefaultAsync(n => n.IdNotificacion == idNotificacion && n.IdPersona == idPersona);
            if (notif != null && !notif.Read)
            {
                notif.Read = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> GetCantidadNoLeidasAsync(int idPersona)
        {
            return await _dbContext.LicitacionNotificaciones
                .CountAsync(n => n.IdPersona == idPersona && !n.Read);
        }
    }
}
