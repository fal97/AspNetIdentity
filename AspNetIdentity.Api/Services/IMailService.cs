using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;


namespace AspNetIdentity.Api.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }

    public class MailService : IMailService
    {
        private IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = _configuration["SendGridAPIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("abijanudara97@gmail.com");
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);

            if(response.IsSuccessStatusCode == false)
            {
                throw new Exception("Email not send");

            }

        }
    }

}
