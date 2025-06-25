using Microsoft.AspNetCore.Mvc;
using Portafolio.Services;
using Portafolio.Models;
using System.Threading.Tasks;

namespace Portafolio.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly GmailServiceHelper _gmailServiceHelper;

        public ContactController(GmailServiceHelper gmailServiceHelper)
        {
            _gmailServiceHelper = new GmailServiceHelper();
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
                await _gmailServiceHelper.SendEmailAsync(model);
                return Ok(new { message = "Email enviado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al enviar el mensaje",
                    error = ex.Message
                });
            }
        }
    }
}

