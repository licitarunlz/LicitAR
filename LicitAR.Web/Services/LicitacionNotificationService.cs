using LicitAR.Core.Services;

namespace LicitAR.Web.Services
{
    public class LicitacionNotificationService : ILicitacionNotificationService
    {
        private readonly EmailSenderService _emailSenderService;

        public LicitacionNotificationService(EmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
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
    }
}
