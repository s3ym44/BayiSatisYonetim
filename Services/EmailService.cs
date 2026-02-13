using MailKit.Net.Smtp;
using MimeKit;

namespace BayiSatisYonetim.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Email:From"] ?? "noreply@bayisatis.com"));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

                using var smtp = new SmtpClient();
                var host = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
                var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
                var user = _configuration["Email:Username"] ?? "";
                var pass = _configuration["Email:Password"] ?? "";

                await smtp.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                if (!string.IsNullOrEmpty(user))
                {
                    await smtp.AuthenticateAsync(user, pass);
                }
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "E-posta gönderilirken hata oluştu: {To}", to);
            }
        }
    }
}
