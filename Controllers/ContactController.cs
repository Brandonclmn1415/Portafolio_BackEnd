using System;
using System.Net;
using System.Net.Mail;
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
                var smtpHost = smtpSettings["Host"];
                var smtpPort = smtpSettings.GetValue<int>("Port");
                var smtpUser = smtpSettings["Username"];
                var smtpPass = smtpSettings["Password"];
                var enableSsl = smtpSettings.GetValue<bool>("EnableSsl");

                Console.WriteLine("FromEmail:"+ _configuration["SmtpSettings:FromEmail"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail),
                    Subject = $"Mensaje de contacto: {model.Subject}",
                    Body = $"Email: {model.Email}\n\nMensaje:\n{model.Message}",
                    IsBodyHtml = false
                };

                mailMessage.To.Add(toEmail);

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = enableSsl;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine("Email enviado con exito.");
                }

                return Ok(new { message = "Mensaje enviado con exito" });
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
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
};

