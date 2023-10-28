using Newtonsoft.Json;

namespace GigaChatSharp.Models
{
    /// <summary>
    /// Параметры запроса к GigaChat.
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Название модели, от которой нужно получить ответ.
        /// </summary>
        [JsonProperty("messages")]
        public MessageQuery[] Messages { get; set; } = new MessageQuery[] { };

        /// <summary>
        /// Название модели, от которой нужно получить ответ.
        /// </summary>
        [JsonProperty("model")]
        public string Model { get; set; } = "GigaChat:latest";

        /// <summary>
        /// Температура выборки в диапазоне от ноля до двух. Чем выше значение, тем более случайным будет ответ модели.
        /// Разрешенное значение от 0...2.
        /// </summary>
        [JsonProperty("temperature")]
        public float Temperature { get; set; } = 0.87f;

        /// <summary>
        /// Параметр используется как альтернатива temperature. Задает вероятностную массу токенов, которые должна учитывать модель. Так, если передать значение 0.1, модель будет учитывать только токены, чья вероятностная масса входит в верхние 10%.
        /// Разрешенное значение от 0...1.
        /// </summary>
        [JsonProperty("top_p")]
        public float TopP { get; set; } = 0.47f;

        /// <summary>
        /// Количество вариантов ответов, которые нужно сгенерировать для каждого входного сообщения.
        /// Разрешенное значение от 1...4.
        /// </summary>
        [JsonProperty("n")]
        public int n { get; set; } = 1;

        /// <summary>
        /// Максимальное количество токенов, которые будут использованы для создания ответов.
        /// </summary>
        [JsonProperty("max_tokens")]
        public int MaxTokens { get; set; } = 512;

        /// <summary>
        /// Количество повторений слов:
        /// При значениях 0 до 1 модель будет повторять уже использованные слова.
        /// Значение 1.0 — нейтральное значение.
        /// При значении больше 1 модель будет стараться не повторять слова.
        /// </summary>
        [JsonProperty("repetition_penalty")]
        public float RepetitionPenalty { get; set; } = 1.07f;
    }
}
