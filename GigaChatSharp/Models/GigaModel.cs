using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class GigaModel
    {
        [JsonProperty("data")]
        public AIModel[] data { get; set; }

        [JsonProperty("object")]
        public string obj { get; set; }
    }
}
