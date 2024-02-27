using Microsoft.AspNetCore.Mvc;
using sampleinvoice.Services;

namespace sampleinvoice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly EmailService _emailService;

        public SendEmailController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("sendPDF")]
        public async Task<IActionResult> SendPDFViaEmail([FromForm] string email, [FromForm] Microsoft.AspNetCore.Http.IFormFile pdfFile)
        {
            try
            {
                if (pdfFile == null || pdfFile.Length == 0)
                {
                    return BadRequest("PDF file is required.");
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest("Email address is required.");
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await pdfFile.CopyToAsync(memoryStream);
                    byte[] pdfBytes = memoryStream.ToArray();

                    // Assuming the PDF content is converted to bytes
                    string subject = "Sample PDF";
                    string body = "Please find the attached PDF.";

                    // Send email with PDF attachment
                    bool isEmailSent = _emailService.SendEmail(email, subject, body, pdfBytes);

                    if (isEmailSent)
                    {
                        return Ok("Email sent successfully.");
                    }
                    else
                    {
                        return StatusCode(500, "Failed to send email.");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}