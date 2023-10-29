using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class ListModel
    {
        [JsonProperty("data")]
        public Model[] data { get; set; }

        [JsonProperty("object")]
        public string obj { get; set; }
    }
}
