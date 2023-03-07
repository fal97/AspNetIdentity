

using AspNetIdentity.Api.Dtos;
using RestSharp.Authenticators;
using RestSharp;
using static System.Net.WebRequestMethods;
using Azure.Core;
using RestSharp.Serialization.Json;
using System.Reflection;

namespace AspNetIdentity.Api.Services
{
    public class MailgunStats
    {
        public int TotalSent { get; set; }
        public int TotalDelivered { get; set; }
        public int TotalOpened { get; set; }
        public int TotalClicked { get; set; }
        public int TotalUnsubscribed { get; set; }
        public int TotalComplained { get; set; }
    }

    public interface IMailGunService
    {
        Task<IRestResponse> SendMailAsync(UserEmailOptions userEmailOptions);


        Task<IRestResponse> GetMailStats(string senderEmail);
    }


    public class MailGunService : IMailGunService
    {
        private readonly IConfiguration _configuration;

        public MailGunService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task<IRestResponse> GetMailStats(string senderEmail)
        {
            string sender = senderEmail;
            var apiKey = "4c7325dc861f19c213d18256d867af7d-ca9eeb88-2cd981b1";
            RestClient client = new RestClient();
            client.BaseUrl = new Uri("https://api.mailgun.net/v3");

            client.Authenticator =
            new HttpBasicAuthenticator("api",
                                       apiKey);
            RestRequest request = new RestRequest(Method.GET);
            request.AddParameter("domain", "sandboxdb77de88cea24c3e9a514c0ce7a44a02.mailgun.org", ParameterType.UrlSegment);
            request.Resource = "{domain}/stats";
            DateTime startDate = new DateTime(2023, 01, 01); // change this to your desired start date
            DateTime endDate = new DateTime(2023, 02, 19); // change this to your desired end date
            request.AddParameter("event", "delivered");
            request.AddParameter("start", startDate.ToString("yyyy-MM-dd"));
            request.AddParameter("end", endDate.ToString("yyyy-MM-dd"));
            request.AddParameter("sender", senderEmail);

            IRestResponse response  = client.Execute(request);

            return (Task<IRestResponse>)response;
            //IRestResponse response = client.Execute(request);

            //if (response.IsSuccessful)
            //{
            //    var deserializer = new JsonDeserializer();
            //    MailgunStats stats = deserializer.Deserialize<MailgunStats>(response);

            //    Console.WriteLine($"Total emails sent by {sender}: {stats.TotalSent}");
            //    Console.WriteLine($"Total emails delivered by {sender}: {stats.TotalDelivered}");
            //    Console.WriteLine($"Total emails opened by {sender}: {stats.TotalOpened}");
            //    Console.WriteLine($"Total emails clicked by {sender}: {stats.TotalClicked}");
            //    Console.WriteLine($"Total unsubscribed by {sender}: {stats.TotalUnsubscribed}");
            //    Console.WriteLine($"Total complaints by {sender}: {stats.TotalComplained}");
            //}
            //else
            //{
            //    Console.WriteLine($"Error: {response.ErrorMessage}");
            //}

            //return response;
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
