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

        public bool SendEmail(string toEmail, string subject, string body, byte[] pdfBytes)
        {
            try
            {
                string smtpServer = _configuration["SmtpSettings:SmtpServer"];
                int smtpPort = _configuration.GetValue<int>("SmtpSettings:SmtpPort");
                string smtpUsername = _configuration["SmtpSettings:SmtpUsername"];
                string smtpPassword = _configuration["SmtpSettings:SmtpPassword"];

                SmtpClient client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                MailMessage message = new MailMessage(smtpUsername, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                using (MemoryStream stream = new MemoryStream(pdfBytes))
                {
                    message.Attachments.Add(new Attachment(stream, "invoice.pdf"));
                    client.Send(message);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while sending email", ex);
            }
        }
    }
}