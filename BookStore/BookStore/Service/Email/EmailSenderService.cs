using System.Net.Mail;
using System.Net;

namespace BookStore.Service.Email
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _config;

        public EmailSenderService(IConfiguration config)
        {
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            string senderEmail = _config["SmtpSettings:SenderEmail"];
            string senderPassword = _config["SmtpSettings:AppPassword"];

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, senderPassword)
            };

            return client.SendMailAsync(
                new MailMessage(from: senderEmail,
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
