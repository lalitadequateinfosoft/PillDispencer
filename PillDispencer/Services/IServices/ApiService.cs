using Newtonsoft.Json;
using PillDispencer.Helper;
using PillDispencer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PillDispencer.Services.IServices
{
    public class ApiService
    {
        public async Task<UserTokens> LoginAsync(LoginModel user)
        {
            UserTokens userTokens = new UserTokens();

            HttpClient client = httphelper.GetClient();
            StringContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
            string endpoint = $"{Urls.Url}" + ApiPath.Login;


            using (var Response = await client.PostAsync(endpoint, content))
            {
                if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var result = await Response.Content.ReadAsStringAsync();
                    userTokens = JsonConvert.DeserializeObject<UserTokens>(result);

                    if (userTokens.Code == (int)ResponseEnum.success)
                    {
                        AppicationSession.SessionDetails = userTokens;
                    }
                }
                else
                {
                    userTokens.Status = "Internal Server Error";
                    userTokens.Code = (int)ResponseEnum.internal_server_error;
                }

            }


            return userTokens;
        }
    }
}
