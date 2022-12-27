using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PillDispencer.Model;

namespace PillDispencer.Helper
{
    public static class httphelper
    {
        public static HttpClient GetClient()
        {
            HttpClientHandler clientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            };
            HttpClient client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
             new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            if (AppicationSession.SessionDetails != null)
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AppicationSession.SessionDetails.Token);
            }


            return client;
        }
    }
}
