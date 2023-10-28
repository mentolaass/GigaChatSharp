using GigaChatSharp.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GigaChatSharp
{
    public class GigaChat
    {
        public string GigaChatClientSecret { get; set; }
        public string GigaChatAuthData { get; set; }
        public TokenAccess DataTokenAccess { get; set; }

        public GigaChat(string ClientSecret, string AuthData, bool AutoUpdateAccessToken = false)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            GigaChatClientSecret = ClientSecret;
            GigaChatAuthData = AuthData;

            if (AutoUpdateAccessToken)
            {
                LoopUpdateAccessToken();
            }
        }

        private async void LoopUpdateAccessToken()
        {
            await Task.Factory.StartNew(async () =>
            {
                for (; ; )
                {
                    if (DateTime.Now < new DateTime(DataTokenAccess.ExpiresDate))
                    {
                        await Authorize();
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public async Task<TokenAccess> Authorize()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth"))
                {
                    message.Headers.Add("Authorization", $"Bearer {GigaChatAuthData}");
                    message.Headers.Add("RqUID", GigaChatClientSecret);

                    message.Content = new StringContent("scope=GIGACHAT_API_PERS");
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await client.SendAsync(message);
                    DataTokenAccess = JsonConvert.DeserializeObject<TokenAccess>(await response.Content.ReadAsStringAsync());

                    return DataTokenAccess;
                }
            }
        }

        public async Task<GigaModel> GetModels()
        {
            if (DateTime.Now < new DateTime(DataTokenAccess.ExpiresDate))
            {
                throw new Exception("Access token is expired... Please create new token...");
            }

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://gigachat.devices.sberbank.ru/api/v1/models"))
                {
                    message.Headers.Add("Authorization", $"Bearer {DataTokenAccess.AccessToken}");

                    var response = await client.SendAsync(message);
                    var data = JsonConvert.DeserializeObject<GigaModel>(await response.Content.ReadAsStringAsync());

                    return data;
                }
            }
        }

        public async Task<ResponseQuery> SendQuery(Parameters dataParams, string answer)
        {
            if (DateTime.Now < new DateTime(DataTokenAccess.ExpiresDate))
            {
                throw new Exception("Access token is expired... Please create new token...");
            }
            else if (dataParams.Model.Length == 0) throw new Exception("Please write model name to send query...");

            dataParams.Messages = dataParams.Messages.Append(new MessageQuery() { Role="user", Content=answer }).ToArray();

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://gigachat.devices.sberbank.ru/api/v1/chat/completions"))
                {
                    // set params
                    message.Headers.Add("Authorization", $"Bearer {DataTokenAccess.AccessToken}");
                    message.Content = new StringContent(JsonConvert.SerializeObject(dataParams), Encoding.UTF8);

                    var response = await client.SendAsync(message);
                    var data = JsonConvert.DeserializeObject<ResponseQuery>(await response.Content.ReadAsStringAsync());

                    return data;
                }
            }
        }

        public TimeSpan GetDateToExpireTokenAccess()
        {
            return new DateTime(DataTokenAccess.ExpiresDate) - DateTime.Now;
        }
    }
}
