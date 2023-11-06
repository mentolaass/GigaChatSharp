using System.Threading.Tasks;

namespace GigaChatSharp.Events.Models
{
    public class AccessTokenArgs
    {
        private GigaChat _GigaChatClient { get; set; }
        public GigaChat GigaChatClient { set { _GigaChatClient = value; } }

        /// <summary>
        /// Выполнить переавторизацию для получения нового токена доступа.
        /// </summary>
        /// <returns></returns>
        public async Task ReAuthorize()
        {
            await _GigaChatClient.Authorize();
        }
    }
}
