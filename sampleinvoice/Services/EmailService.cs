using System.Net;
using System.Net.Mail;

namespace sampleinvoice.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool SendEmail(string toEmail, string subject, string body)
        {
            try
            {
                // Read SMTP settings from configuration
                string smtpServer = _configuration["SmtpSettings:SmtpServer"];
                int smtpPort = _configuration.GetValue<int>("SmtpSettings:SmtpPort");
                string smtpUsername = _configuration["SmtpSettings:SmtpUsername"];
                string smtpPassword = _configuration["SmtpSettings:SmtpPassword"];

                // Set up the SMTP client
                SmtpClient client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                // Create and send the email
                MailMessage message = new MailMessage(smtpUsername, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                client.Send(message);

                // Return true indicating the email was sent successfully
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while sending email", ex);
            }
        }
    }
}