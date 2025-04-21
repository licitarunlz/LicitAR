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
            /*var smtpClient = new SmtpClient
            {
                Host = _configuration["Email:Smtp:Host"],
                Port = int.Parse(_configuration["Email:Smtp:Port"]),
                EnableSsl = true,
                Credentials = new NetworkCredential(
                    _configuration["Email:Smtp:Username"],
                    _configuration["Email:Smtp:Password"])
            };

            var message = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Smtp:From"]),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };*/
             
            using (var client = new SmtpClient())
            {
                client.Host = _configuration["Email:Smtp:Host"];
                client.Port = int.Parse(_configuration["Email:Smtp:Port"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(
                    _configuration["Email:Smtp:Username"],
                    _configuration["Email:Smtp:Password"]);


                using (var message = new MailMessage(
                    from: new MailAddress(_configuration["Email:Smtp:From"], "LicitAR"),
                    to: new MailAddress(email, "")
                    ))
                {

                    message.Subject = subject;
                    message.Body = htmlMessage;

                    message.IsBodyHtml = true;

                    client.Send(message);
                }
            }
        }
    }
}
