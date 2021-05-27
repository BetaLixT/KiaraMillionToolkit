using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace messageParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var lines = System.IO.File.ReadAllLines("messages.txt");
            var messages = new List<Message>();

            bool isMessage = true;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (isMessage)
                {
                    messages.Add(new Message 
                    { 
                        message = line,
                        width = 1,
                        height = 1,
                    });
                }
                else 
                {
                    messages.Last().author = line.Substring(1).Trim();
                }
                isMessage = !isMessage;
            }

            System.IO.File.WriteAllText("messages.json", JsonConvert.SerializeObject(messages));
        }
    }

    public class Message
    {
        public float width { get; set; }
        public float height { get; set; }
        public string message { get; set; }
        public string author { get; set; }
    }
}
