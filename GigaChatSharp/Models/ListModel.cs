using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace GigaChatSharp.Models
{
    public class ListModel
    {
        [JsonProperty("data")]
        private Model[] data { get; }

        [JsonProperty("object")]
        public string obj { get; set; }

        public Model this[int x]
        {
            get { return data[x]; }
        }

        public List<Model> ToList() => data.ToList();
    }
}
