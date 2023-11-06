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

        /// <summary>
        /// Вызывается по истечении срока токена доступа.
        /// </summary>
        public event AccessTokenExpired AccessTokenExpiredHandler;

        private delegate void CallbackFunc();

        private event CallbackFunc CallbackFuncHandler;

        #endregion events

        /// <summary>
        /// Основной класс для быстрого доступа к GigaChat AI.
        /// </summary>
        /// <param name="clientSecret"></param>
        /// <param name="authData"></param>
        /// <param name="scope"></param>
        public GigaChat(string clientSecret, string authData, Scope scope)
        {
            ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(delegate { return true; });

            GigaChatClientSecret = clientSecret;
            GigaChatAuthData = authData;
            GigaChatScope = scope;

            CallbackFuncHandler += GigaChat_CallbackFuncHandler;
        }

        private readonly DateTime UnixEpoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        /// <summary>
        /// Получить точное UNIX-время истечения токена доступа, укажите необязательный параметр для преобразования в текущий часовой пояс.
        /// </summary>
        /// <returns></returns>
        public DateTime GetAccessTokenExpDate(bool toLocal = false)
        {
            return toLocal ? UnixEpoch.AddMilliseconds(DataTokenAccess.ExpiresDate).ToLocalTime() : UnixEpoch.AddMilliseconds(DataTokenAccess.ExpiresDate);
        }

        /// <summary>
        /// Ручной вызов AccessTokenExpired.
        /// </summary>
        public void InvokeATEX()
        {
            AccessTokenExpiredHandler.Invoke(new AccessTokenArgs() { GigaChatClient = this });
        }

        private void HandleAccessTokenExpire()
        {
            if ((long)DateTime.UtcNow.Subtract(DateTime.Now).TotalMilliseconds >= DataTokenAccess.ExpiresDate)
            {
                IsAuthState = true; InvokeATEX();
            }
        }

        private void GigaChat_CallbackFuncHandler()
        {
            if (DataTokenAccess is null) // Если не проводилась авторизация.
            {
                throw new Exception("Please auth to use this api.");
            }
        }

        /// <summary>
        /// Выполнить авторизацию для получения AccessToken. (Рекомендуется выполнять привязку на ивент AccessTokenExpired, для обработки завершение времени использования токена.)
        /// </summary>
        /// <returns></returns>
        public async Task Authorize()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://ngw.devices.sberbank.ru:9443/api/v2/oauth"))
                {
                    // Установка заголовков для запроса
                    message.Headers.Add("Authorization", $"Bearer {GigaChatAuthData}");
                    message.Headers.Add("RqUID", GigaChatClientSecret);
                    // Установка содержимого запроса
                    message.Content = new StringContent($"scope={GigaChatScope}");
                    message.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    // Отправка асинхронного запроса и получение ответа
                    var response = await client.SendAsync(message);

                    // Десериализация ответа в объект TokenAccessModel
                    var data = await response.Content.ReadAsStringAsync();
                    DataTokenAccess = JsonConvert.DeserializeObject<TokenAccessModel>(data);

                    // Установка значения переменной IsAuthState в false, для остановки процесса авторизации
                    IsAuthState = false;

                    client.Dispose();  // Освобождение ресурсов HttpClient
                    message.Dispose();  // Освобождение ресурсов HttpRequestMessage
                }
            }
        }

        /// <summary>
        /// Получить список действительных моделей AI GigaChat, может вернуть null, если токен доступа истек.
        /// </summary>
        /// <returns></returns>
        public async Task<ListModel> GetModels()
        {
            CallbackFuncHandler.Invoke();
            // Вызов функции обратного вызова для обработки события.

            HandleAccessTokenExpire();
            // Обработка истечения срока действия токена доступа.

            if (IsAuthState)
                return null; // Возвращаем null, если процесс авторизации уже выполняется, чтобы избежать дублирования запросов.

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://gigachat.devices.sberbank.ru/api/v1/models"))
                {
                    message.Headers.Add("Authorization", $"Bearer {DataTokenAccess.AccessToken}");
                    // Добавление заголовка авторизации с использованием токена доступа.

                    var response = await client.SendAsync(message);
                    // Отправка асинхронного запроса и получение ответа.

                    var data = JsonConvert.DeserializeObject<ListModel>(await response.Content.ReadAsStringAsync());
                    // Десериализация содержимого ответа в объект ListModel.

                    client.Dispose(); // Освобождение ресурсов HttpClient
                    message.Dispose(); // Освобождение ресурсов HttpRequestMessage

                    return data;
                }
            }
        }

        /// <summary>
        /// Отправка запроса по параметрам, может вернуть null, если токен доступа истек.
        /// </summary>
        /// <param name="dataParams"></param>
        /// <param name="answer"></param>
        /// <returns></returns>
        public async Task<ResponseQueryModel> SendMessage(ParametersModel dataParams, string answer)
        {
            CallbackFuncHandler.Invoke();
            // Вызов функции обратного вызова для обработки события.

            HandleAccessTokenExpire();
            // Обработка истечения срока действия токена доступа.

            if (IsAuthState)
                return null; // Возвращаем null, если процесс авторизации уже выполняется, чтобы избежать ошибки запроса.

            dataParams.Messages = dataParams.Messages.Append(new MessageQueryModel() { Role = "user", Content = answer }).ToArray();
            // Добавление нового сообщения пользователя в массив сообщений dataParams.

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "https://gigachat.devices.sberbank.ru/api/v1/chat/completions"))
                {
                    message.Headers.Add("Authorization", $"Bearer {DataTokenAccess.AccessToken}");
                    // Добавление заголовка авторизации с использованием токена доступа.

                    message.Content = new StringContent(JsonConvert.SerializeObject(dataParams), Encoding.UTF8);
                    // Установка содержимого запроса как сериализованный объект dataParams.

                    var response = await client.SendAsync(message);
                    // Отправка асинхронного запроса и получение ответа.

                    var data = JsonConvert.DeserializeObject<ResponseQueryModel>(await response.Content.ReadAsStringAsync());
                    // Десериализация содержимого ответа в объект ResponseQueryModel.

                    client.Dispose(); // Освобождение ресурсов HttpClient
                    message.Dispose(); // Освобождение ресурсов HttpRequestMessage

                    return data;
                }
            }
        }
    } 
}
