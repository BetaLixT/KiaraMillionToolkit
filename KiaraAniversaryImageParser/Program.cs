using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

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
                var message = messages.Where(x => x.artist == response.Name).FirstOrDefault();
                if (message == null)
                {
                    messages.Add(new Message{
                        message = response.Message,
                        artist = response.Name,
                        width = 1,
                        height = 1,
                        src = string.Empty
                    });
                }
                else
                {
                    messages[messages.IndexOf(message)].message = response.Message;
                }
            }
            foreach (var message in messages)
            {
                if (string.IsNullOrEmpty(message.src))
                {
                    
                }
            }

            System.IO.File.WriteAllText("messages.json", JsonConvert.SerializeObject(messages));
        }
    }

    public class Response
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Message you want to attach (optional)")]
        public string Message { get; set; }
    }

    public class Message
    {
        public float width { get; set; }
        public float height { get; set; }
        public string message { get; set; }
        public string artist { get; set; }
        public string src { get; set; }
    }
}
