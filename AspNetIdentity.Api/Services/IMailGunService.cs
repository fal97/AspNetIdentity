

using AspNetIdentity.Api.Dtos;
using RestSharp.Authenticators;
using RestSharp;
using static System.Net.WebRequestMethods;

namespace AspNetIdentity.Api.Services
{
    public interface IMailGunService
    {
        Task<IRestResponse> SendMailAsync(UserEmailOptions userEmailOptions);
    }


    public class MailGunService : IMailGunService
    {
        private readonly IConfiguration _configuration;

        public MailGunService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IRestResponse> SendMailAsync(UserEmailOptions userEmailOptions)
        {
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");           

            client.Authenticator =
            new HttpBasicAuthenticator("api",
                                       "4c7325dc861f19c213d18256d867af7d-ca9eeb88-2cd981b1");
            RestRequest request = new RestRequest();
            request.AddParameter("domain", "sandboxdb77de88cea24c3e9a514c0ce7a44a02.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Mailgun Sandbox <postmaster@sandboxdb77de88cea24c3e9a514c0ce7a44a02.mailgun.org>");
            request.AddParameter("to", "Abijan Padmathilaka <abijanudara97@gmail.com>");
            request.AddParameter("subject", userEmailOptions.Subject);
            request.AddParameter("text", userEmailOptions.Body);
            request.Method = Method.POST;
            return client.Execute(request);


        }
    }
}
