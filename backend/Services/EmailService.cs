using System.Net;
using System.Net.Mail;
using backend.Data;
using backend.Interfaces;
using backend.Models;
using Serilog;

namespace backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailBodyService _emailBodyService;
        private readonly string? _baseUrl;

        public EmailService(
            ApplicationDbContext context,
            IConfiguration configuration,
            IEmailBodyService emailBodyService)
        {
            _configuration = configuration;
            _context = context;
            _emailBodyService = emailBodyService;
            _baseUrl = _configuration["EmailSettings:ActivationURL"];
        }

        public async Task SendActivationEmail(string userId, string token)
        {
            var encodedToken = WebUtility.UrlEncode(token);
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var name = $"{user.FirstName} {user.LastName}";
                var email = user.Email;

                var activationLink = $"{_baseUrl}/auth/activate?userId={userId}&token={encodedToken}";
                var subject = "Aktywacja konta ZION SIGID";
                string emailBody = _emailBodyService.ActivationEmailBody(name, activationLink);

                if (!string.IsNullOrEmpty(email))
                {
                    await SendEmail(email, subject, emailBody);
                }
            }
        }

        public async Task SendTwoFactorCodeEmail(string userId, string twoFactorCode)
        {
            var user = _context.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (user != null)
            {
                var name = $"{user.FirstName} {user.LastName}";
                var email = user.Email;

                var subject = "Kod do dwuetapowej weryfikacji ZION SIGID";
                string emailBody = _emailBodyService.TwoFactorCodeEmailBody(name, twoFactorCode);

                if (!string.IsNullOrEmpty(email))
                {
                    await SendEmail(email, subject, emailBody);
                }
            }
        }

        private async Task SendEmail(string to, string subject, string body)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var smtpServer = emailSettings["SmtpServer"];
            var smtpPort = Convert.ToInt32(emailSettings["SmtpPort"]);
            var smtpUsername = emailSettings["SmtpUsername"];
            var smtpPassword = emailSettings["SmtpPassword"];

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(smtpUsername ?? string.Empty),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(to);

            var smtpClient = new SmtpClient(smtpServer)
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                EnableSsl = true,
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Wystąpił błąd przy wysyłaniu emaila: {subject}.");
            }
        }
    }
}