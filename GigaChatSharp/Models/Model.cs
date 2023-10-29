using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class Model
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("object")]
        public string obj { get; set; }

        [JsonProperty("owned_by")]
        public string owner { get; set; }
    }
}
