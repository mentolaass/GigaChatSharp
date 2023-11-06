using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class TokenAccessModel
    {
        /// <summary>
        /// Токен доступа.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Миллисекунды даты истечения.
        /// </summary>
        [JsonProperty("expires_at")]
        public long ExpiresDate { get; set; }
    }
}
