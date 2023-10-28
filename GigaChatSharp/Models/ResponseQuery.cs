using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class ResponseQuery
    {
        /// <summary>
        /// Массив ответов модели.
        /// </summary>
        [JsonProperty("choices")]
        public MessageModel[] Messages { get; set; }

        /// <summary>
        /// Дата и время создания ответа в формате Unix time
        /// </summary>
        [JsonProperty("created")]
        public long DateCreated { get; set; }

        /// <summary>
        /// Название модели, которая вернула ответ
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; }

        /// <summary>
        /// Данные об использовании модели
        /// </summary>
        [JsonProperty("usage")]
        public MessageParameters Parameters { get; set; }

        /// <summary>
        /// Название вызываемого метода
        /// </summary>
        [JsonProperty("object")]
        public string TypeMethod { get; set; }
    }
}
