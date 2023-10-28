using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class AIModel
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string obj { get; set; }

        [JsonProperty("owned_by")]
        public string owner { get; set; }
    }
}
