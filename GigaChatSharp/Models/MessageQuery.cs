using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class MessageQuery
    {
        /// <summary>
        /// Роль автора сообщения
        /// </summary>
        [JsonProperty("role")]
        public string Role { get; set; }

        /// <summary>
        /// Текст сообщения
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
