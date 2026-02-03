using System.Net;
using System.Net.Mail;

namespace DependencySystem.Services.Auth
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtp = _config.GetSection("SmtpSettings");

                var client = new SmtpClient(smtp["Host"])
                {
                    Port = int.Parse(smtp["Port"]!),
                    Credentials = new NetworkCredential(smtp["Username"], smtp["Password"]),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtp["SenderEmail"]!, smtp["SenderName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                throw new Exception("SMTP Failed: " + ex.Message);
            }
        }

    }
}
