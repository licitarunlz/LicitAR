using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace LicitAR.Web.Services
{
    public class EmailSenderService
    {
        private readonly IViewRenderService _viewRenderService;
        private readonly SmtpClient _smtpClient;
        private readonly string _fromAddress;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(
            IViewRenderService viewRenderService,
            IConfiguration config,
            ILogger<EmailSenderService> logger)
        {
            _viewRenderService = viewRenderService;
            _fromAddress = config["Email:Smtp:From"];
            _smtpClient = new SmtpClient(config["Email:Smtp:Host"], int.Parse(config["Email:Smtp:Port"]))
            {
                Credentials = new NetworkCredential(
                    config["Email:Smtp:Username"],
                    config["Email:Smtp:Password"]
                ),
                EnableSsl = true // Always enable SSL
            };
            _logger = logger;
        }

        public async Task SendLicitacionCreadaAsync(string to, object model)
        {
            try
            {
                var html = await _viewRenderService.RenderToStringAsync(
                    "LicitacionCreada", model);

                var from = new MailAddress(_fromAddress, "LicitAR");

                var mail = new MailMessage(from, new MailAddress(to))
                {
                    Subject = "Nueva Licitación Creada",
                    Body = html,
                    IsBodyHtml = true
                };
                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando email de creación de licitación a {to}");
            }
        }

        public async Task SendInvitacionLicitacionAsync(string to, object model)
        {
            try
            {
                var html = await _viewRenderService.RenderToStringAsync(
                    "InvitacionLicitacion", model);

                var from = new MailAddress(_fromAddress, "LicitAR");

                var mail = new MailMessage(from, new MailAddress(to))
                {
                    Subject = "Te han invitado a una licitación",
                    Body = html,
                    IsBodyHtml = true
                };
                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando invitación de licitación a {to}");
            }
        }

        public async Task SendLicitacionImpugnadaAsync(string to, object model)
        {
            try
            {
                var html = await _viewRenderService.RenderToStringAsync(
                    "LicitacionImpugnada", model);

                var from = new MailAddress(_fromAddress, "LicitAR");

                var mail = new MailMessage(from, new MailAddress(to))
                {
                    Subject = "Licitación impugnada",
                    Body = html,
                    IsBodyHtml = true
                };
                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando email de licitación impugnada a {to}");
            }
        }

        public async Task SendLicitacionAdjudicadaAsync(string to, object model)
        {
            try
            {
                var html = await _viewRenderService.RenderToStringAsync(
                    "LicitacionAdjudicada", model);

                var from = new MailAddress(_fromAddress, "LicitAR");

                var mail = new MailMessage(from, new MailAddress(to))
                {
                    Subject = "¡Felicitaciones! Licitación adjudicada",
                    Body = html,
                    IsBodyHtml = true
                };
                await _smtpClient.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando email de licitación adjudicada a {to}");
            }
        }
    }
}
