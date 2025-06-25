using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using System.Text;
using System.Threading.Tasks;
using Portafolio.Models;
using Google.Apis.Util.Store;

namespace Portafolio.Services
{
    public class GmailServiceHelper
    {
        private readonly string[] Scopes = { GmailService.Scope.GmailSend };
        private readonly string ApplicationName = "Portafolio Contact";

        public async Task SendEmailAsync(ContactFormModel model)
        {
            var credentialPath = "credentials.json"; 

            UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromFile(credentialPath).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore("token.json", true));

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(model.Email));
            message.To.Add(MailboxAddress.Parse("brrandonlol1415@gmail.com")); 
            message.Subject = model.Subject;
            message.Body = new TextPart("plain") { Text = model.Message };

            using var stream = new MemoryStream();
            await message.WriteToAsync(stream);
            var rawMessage = Convert.ToBase64String(stream.ToArray())
                .Replace('+', '-').Replace('/', '_').Replace("=", "");

            var gmailMessage = new Message { Raw = rawMessage };
            await service.Users.Messages.Send(gmailMessage, "me").ExecuteAsync();
        }
    }
}