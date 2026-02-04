using System.Net;
using System.Net.Mail;

namespace DependencySystem.Services.Auth
{
    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailService()
        {
            _host = Environment.GetEnvironmentVariable("SMTP_HOST")
                    ?? throw new Exception("SMTP_HOST not set");

            _port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")
                    ?? throw new Exception("SMTP_PORT not set"));

            _username = Environment.GetEnvironmentVariable("SMTP_USERNAME")
                    ?? throw new Exception("SMTP_USERNAME not set");

            _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                    ?? throw new Exception("SMTP_PASSWORD not set");

            _senderEmail = Environment.GetEnvironmentVariable("SMTP_SENDER_EMAIL")
                    ?? throw new Exception("SMTP_SENDER_EMAIL not set");

            _senderName = Environment.GetEnvironmentVariable("SMTP_SENDER_NAME")
                    ?? "Dependency System";
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var client = new SmtpClient(_host)
                {
                    Port = _port,
                    Credentials = new NetworkCredential(_username, _password),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
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
