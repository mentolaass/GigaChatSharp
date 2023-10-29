using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    public class MessageParametersModel
    {
        /// <summary>
        /// Количество токенов во входящем сообщении
        /// </summary>
        [JsonProperty("prompt_tokens")]
        public int PromptTokens { get; set; }

        /// <summary>
        /// Количество токенов, сгенерированных моделью
        /// </summary>
        [JsonProperty("completion_tokens")]
        public int CompletionTokens { get; set; }

        /// <summary>
        /// Общее количество токенов
        /// </summary>
        [JsonProperty("total_tokens")]
        public int TotalTokens { get; set; }
    }
}
