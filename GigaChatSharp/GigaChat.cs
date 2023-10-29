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
        private bool IsAuthState { get; set; }
        private TokenAccessModel DataTokenAccess { get; set; }
        # endregion fields 

        #region events
        public delegate void AccessTokenExpired(AccessTokenArgs args);
        public event AccessTokenExpired AccessTokenExpiredHandler;

        private delegate void CallbackFunc();
        private event CallbackFunc CallbackFuncHandler;
        #endregion events

        public GigaChat(string clientSecret, string authData, Scope scope)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            GigaChatClientSecret = clientSecret;
            GigaChatAuthData = authData;
            GigaChatScope = scope;

            CallbackFuncHandler += GigaChat_CallbackFuncHandler;
        }

        private void GigaChat_CallbackFuncHandler()
        {
            if (DataTokenAccess is null)
            {
                throw new Exception("Please auth to use this api.");
            }
        }

        private readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public DateTime GetAccessTokenExpDate()
        {
            return UnixEpoch.AddMilliseconds(DataTokenAccess.ExpiresDate);
        }

        public async Task Authorize()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth"))
                {
                    message.Headers.Add("Authorization", $"Bearer {GigaChatAuthData}");
                    message.Headers.Add("RqUID", GigaChatClientSecret);
                    message.Content = new StringContent($"scope={GigaChatScope}");
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                    var response = await client.SendAsync(message);
                    DataTokenAccess = JsonConvert.DeserializeObject<TokenAccessModel>(await response.Content.ReadAsStringAsync());
                    IsAuthState = false;
                }
            }
        }

        private void HandleAccessTokenExpire()
        {
            if ((long)DateTime.UtcNow.Subtract(DateTime.Now).TotalMilliseconds >= DataTokenAccess.ExpiresDate)
            {
                IsAuthState = true;
                AccessTokenExpiredHandler.Invoke(new AccessTokenArgs() { GigaChatClient = this });
            }
        }

        public async Task<ListModel> GetModels()
        {
            CallbackFuncHandler.Invoke();
            HandleAccessTokenExpire();
            if (IsAuthState)
                return null;

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
            CallbackFuncHandler.Invoke();
            HandleAccessTokenExpire();
            if (IsAuthState)
                return null;

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
