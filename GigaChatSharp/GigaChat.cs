using GigaChatSharp.Events.Models;
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
        # region fields 
        private string GigaChatClientSecret { get; set; }
        private string GigaChatAuthData { get; set; }
        private Scope GigaChatScope { get; set; }
        private TokenAccessModel DataTokenAccess { get; set; }
        # endregion fields 

        #region events
        public delegate void AccessTokenExpired(AccessTokenArgs args);

        public event AccessTokenExpired AccessTokenExpiredHandler;                      
        #endregion events

        public GigaChat(string clientSecret, string authData, Scope scope)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            GigaChatClientSecret = clientSecret;
            GigaChatAuthData = authData;
            GigaChatScope = scope;
        }

        private bool IsTokenActive()
        {
            return DateTime.Now > new DateTime(DataTokenAccess.ExpiresDate);
        }

        public async Task Authorize()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth"))
                {
                    message.Headers.Add("Authorization", $"Bearer {GigaChatAuthData}");
                    message.Headers.Add("RqUID", GigaChatClientSecret);

                    message.Content = new StringContent($"scope={GigaChatScope.ToString()}");
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    var response = await client.SendAsync(message);
                    DataTokenAccess = JsonConvert.DeserializeObject<TokenAccessModel>(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<ListModel> GetModels()
        {
            if (!IsTokenActive())
            {
                AccessTokenExpiredHandler.Invoke(new AccessTokenArgs() { GigaChatClient = this });
                return null;
            }

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://gigachat.devices.sberbank.ru/api/v1/models"))
                {
                    message.Headers.Add("Authorization", $"Bearer {DataTokenAccess.AccessToken}");

                    var response = await client.SendAsync(message);
                    var data = JsonConvert.DeserializeObject<ListModel>(await response.Content.ReadAsStringAsync());

                    return data;
                }
            }
        }

        public async Task<ResponseQueryModel> SendMessage(ParametersModel dataParams, string answer)
        {
            if (!IsTokenActive())
            {
                AccessTokenExpiredHandler.Invoke(new AccessTokenArgs() { GigaChatClient = this });
                return null;
            }

            dataParams.Messages = dataParams.Messages.Append(new MessageQueryModel() { Role="user", Content=answer }).ToArray();

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://gigachat.devices.sberbank.ru/api/v1/chat/completions"))
                {
                    message.Headers.Add("Authorization", $"Bearer {DataTokenAccess.AccessToken}");
                    message.Content = new StringContent(JsonConvert.SerializeObject(dataParams), Encoding.UTF8);

                    var response = await client.SendAsync(message);
                    var data = JsonConvert.DeserializeObject<ResponseQueryModel>(await response.Content.ReadAsStringAsync());

                    return data;
                }
            }
        }
    } 
}
