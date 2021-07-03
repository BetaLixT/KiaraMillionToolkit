using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace KiaraBirthdayMessageParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var rand = new Random();
            var responses = JsonConvert.DeserializeObject<List<Response>>(System.IO.File.ReadAllText("responses.json"));
            var messages = JsonConvert.DeserializeObject<List<Message>>(System.IO.File.ReadAllText("messages.json"));

            foreach (var response in responses)
            {
                var message = messages.Where(x => x.author == response.Name).FirstOrDefault();
                if (message == null)
                {
                    messages.Add(new Message{
                        message = string.IsNullOrWhiteSpace(response.Message) ? response.MessageJp : response.Message,
                        author = response.Name,
                        width = 1,
                        height = rand.Next(85, 100) / 100f
                    });
                }
                else
                {
                    // messages[messages.IndexOf(message)].message = string.IsNullOrWhiteSpace(response.Message) ? response.MessageJp : response.Message;
                }
            }

            System.IO.File.WriteAllText("messages.json", JsonConvert.SerializeObject(messages));
        }
    }

    public class Response
    {
        [JsonProperty("Name / お名前")]
        public string Name { get; set; }

        [JsonProperty("Message for EN/GER / 英語/ドイツ語用メッセージ記入欄")]
        public string Message { get; set; }

        [JsonProperty("Message for JP / 日本語用メッセージ記入欄")]
        public string MessageJp { get; set; }
    }

    public class Message
    {
        public float width { get; set; }
        public float height { get; set; }
        public string message { get; set; }
        public string author { get; set; }
    }
}
