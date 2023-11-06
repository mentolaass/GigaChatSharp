using GigaChatSharp;
using GigaChatSharp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GigaChatSharpExample
{
    class Program
    {
        const string secret = "c3c4a22d-f4c2-4fae-9e88-c73821f7009d";

        const string auth = "YjJjOTgwMzAtOTI2NC00MDM5LTk4YmMtMmU0OTNlNzQ4ZGQ2OmMzYzRhMjJkLWY0YzItNGZhZS05ZTg4LWM3MzgyMWY3MDA5ZA==";

        static ParametersModel dataParams = new ParametersModel();

        static async Task Main(string[] args)
        {
            var GigaChatClient = new GigaChat(secret, auth, Scope.GIGACHAT_API_PERS);
            GigaChatClient.AccessTokenExpiredHandler += GigaChatClient_AccessTokenExpiredHandler;

            // Авторизация
            await GigaChatClient.Authorize();

            while (true)
            {
                Console.WriteLine("Введите Ваш запрос: ");
                var answer = Console.ReadLine();

                if (answer.Length != 0)
                {
                    // Отправка запроса
                    try
                    {
                        var response = await GigaChatClient.SendMessage(dataParams, answer);

                        if (response != null)
                        {
                            // Получение ответа
                            var lastmsg = response.Messages[response.Messages.Length - 1];

                            // Вывод ответа
                            Console.WriteLine($"[GIGACHAT] => {lastmsg.Message.Content}");

                            // Запись ответа в историю, чтобы GigaChat запоминал ранее отправленные сообщения.
                            dataParams.Messages.Append(lastmsg.Message);
                        }
                        else
                            Console.Write("Идет переавторизация подождите...");

                    } catch (Exception e) { Console.WriteLine(e.Message); }
                }
            }
        }

        private static async void GigaChatClient_AccessTokenExpiredHandler(GigaChatSharp.Events.Models.AccessTokenArgs args)
        {
            await args.ReAuthorize();
        }
    }
}
