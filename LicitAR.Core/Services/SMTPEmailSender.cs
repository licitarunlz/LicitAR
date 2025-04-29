using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LicitAR.Core.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public SmtpEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient())
            {
                client.Host = _configuration["Email:Smtp:Host"] ?? "defaultHost";
                client.Port = int.Parse(_configuration["Email:Smtp:Port"] ?? "0");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(
                    _configuration["Email:Smtp:Username"] ?? "defaultUsername",
                    _configuration["Email:Smtp:Password"] ?? "defaultPassword");

                using (var message = new MailMessage(
                    from: new MailAddress(_configuration["Email:Smtp:From"] ?? "default@example.com", "LicitAR"),
                    to: new MailAddress(email ?? "default@example.com", "")
                    ))
                {
                    message.Subject = subject ?? "Default Subject";
                    message.Body = htmlMessage ?? "Default Body";

                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
        }
    }
}
