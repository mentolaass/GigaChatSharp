using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class MessageModel
    {
        /// <summary>
        /// Сгенерированное сообщение
        /// </summary>
        [JsonProperty("message")]
        public MessageQuery Message { get; set; }

        /// <summary>
        /// Индекс сообщения в массиве начиная с нуля.
        /// </summary>
        [JsonProperty("index")]
        public int Index { get; set; }

        /// <summary>
        /// Причина завершения гипотезы. Например, stop сообщает, что модель закончила формировать гипотезу и вернула полный ответ.
        /// </summary>
        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }
    }
}
