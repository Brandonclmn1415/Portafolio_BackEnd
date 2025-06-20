using System;
using System.Net;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Portafolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ContactController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost]     
        public async Task<IActionResult> SendEmail([FromBody] ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            try
            {
                var smtpSettings = _configuration.GetSection("SmtpSettings");

                var fromEmail = smtpSettings["FromEmail"];
                var toEmail = smtpSettings["ToEmail"];
                var smtpUser = smtpSettings["Username"];
                var smtpPass = smtpSettings["Password"];
                var smtpHost = smtpSettings["Host"];
                var smtpPort = int.Parse(smtpSettings["Port"] ?? "587");
                var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Portafolio", fromEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = model.Subject;

                message.Body = new TextPart("plain")
                {
                    Text = $"Email: {model.Email}\n\nMessage:\n{model.Message}"
                };

                using var client = new SmtpClient();
                await client.ConnectAsync(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return Ok(new {message = "Mensaje enviado con exito"});
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el email: {ex.Message}");
                return StatusCode(500, new
                {
                    message = "Error al enviar el mensaje",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }        
    }


    public class ContactFormModel
    {
        public required string Email { get; set; }
        public required string Subject { get; set; }
        public required string Message { get; set; }
    }
};

