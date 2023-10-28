using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class TokenAccess
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresDate { get; set; }
    }
}
