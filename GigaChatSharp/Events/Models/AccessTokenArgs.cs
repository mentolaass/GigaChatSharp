using System.Threading.Tasks;

namespace GigaChatSharp.Events.Models
{
    public class AccessTokenArgs
    {
        private GigaChat _GigaChatClient { get; set; }
        public GigaChat GigaChatClient { set { _GigaChatClient = value; } }

        public async Task ReAuthorize()
        {
            await _GigaChatClient.Authorize();
        }
    }
}
