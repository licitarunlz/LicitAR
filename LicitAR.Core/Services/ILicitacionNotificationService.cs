namespace LicitAR.Core.Services
{
    public interface ILicitacionNotificationService
    {
        Task NotificarInvitacionLicitacionAsync(string email, object model);
        Task NotificarLicitacionImpugnadaAsync(string email, object model);
        Task NotificarLicitacionAdjudicadaAsync(string email, object model);
    }
}
