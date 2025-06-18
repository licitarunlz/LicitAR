using LicitAR.Core.Data.Models;

namespace LicitAR.Core.Services
{
    public interface ILicitacionNotificationService
    {
        Task NotificarInvitacionLicitacionAsync(string email, object model);
        Task NotificarLicitacionImpugnadaAsync(string email, object model);
        Task NotificarLicitacionAdjudicadaAsync(string email, object model);
        Task CrearNotificacionAsync(LicitacionNotificacion notificacion);
        Task<List<LicitacionNotificacion>> GetNotificacionesPorUsuarioAsync(int idPersona, int max = 10);
        Task MarcarComoLeidaAsync(int idNotificacion, int idPersona);
        Task<int> GetCantidadNoLeidasAsync(int idPersona);
    }
}
