

using AspNetIdentity.Api.Dtos;
using RestSharp.Authenticators;
using RestSharp;

namespace AspNetIdentity.Api.Services
{
    public interface IMailGunService
    {
        Task SendMailAsync(UserEmailOptions userEmailOptions);
    }


    public class MailGunService : IMailGunService
    {
        private const string APIKey = "xxxxxxxxxxxxxxx-xxxxx-xxxxx";
        private const string BaseUri = "https://api.mailgun.net/v3";
        private const string Domain = "xxxxxx.xxx";
        private const string SenderAddress = "abijanudara97@gmail.com";
        private const string SenderDisplayName = "Abijan";
        private const string Tag = "sampleTag";

        public async Task SendMailAsync(UserEmailOptions userEmailOptions)
        {
            RestClient client = new RestClient
            {
                BaseUrl = new Uri(BaseUri),
                Authenticator = new HttpBasicAuthenticator("api", APIKey)
            };

            RestRequest request = new RestRequest();
            request.AddParameter("domain", Domain, ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", $"{SenderDisplayName} <{SenderAddress}>");

            foreach (var toEmail in userEmailOptions.ToEmails)
            {
                request.AddParameter("to", toEmail);
            }

            request.AddParameter("subject", userEmailOptions.Subject);
            request.AddParameter("html", userEmailOptions.Body);
            request.AddParameter("o:tag", Tag);
            request.Method = Method.POST;
            client.Execute(request);

           
        }
    }
}
