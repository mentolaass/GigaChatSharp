using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class Model
    {
        /// <summary>
        /// Айди модели.
        /// </summary>
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string obj { get; set; }

        /// <summary>
        /// Владелец модели.
        /// </summary>
        [JsonProperty("owned_by")]
        public string owner { get; set; }
    }
}
