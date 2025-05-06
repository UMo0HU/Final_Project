using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using BookStore.Models;
using System.Security.Claims;

namespace BookStore.Service.Email
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClaimsPrincipal _claims;

        public EmailSenderService(IConfiguration config, UserManager<User> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _claims = _httpContextAccessor.HttpContext?.User;
        }
        public Task SendEmailAsync(string email, string subject, string message, bool IsHtml = false)
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
                                )
                { IsBodyHtml = IsHtml});
        }

        public Task ContactUsAsync(string subject, string message)
        {
            if (_claims != null && _claims.Identity.IsAuthenticated)
            {
                var user = _userManager.GetUserAsync(_claims);
                string senderEmail = _config["SmtpSettings:SenderEmail"];
                string senderPassword = _config["SmtpSettings:AppPassword"];

                string template = $@"<p>User With This Email: {user.Result.Email} & Username: {user.Result.UserName}, Sent This Message:<br>{message}</p>";


                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, senderPassword)
                };

                return client.SendMailAsync(
                    new MailMessage(from: senderEmail,
                                    to: senderEmail,
                                    subject,
                                    template
                                    )
                    { IsBodyHtml = true });
            }
            throw new ArgumentException("User Not Signed In.");
        }
    }
}
