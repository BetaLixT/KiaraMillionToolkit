using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace KiaraMilMessageDuplicateCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            var messages = JsonConvert.DeserializeObject<List<Message>>(System.IO.File.ReadAllText("messages.json"));
            var secondList = new List<Message>();
            var duplicateMessage = new List<Message>();
            var duplicateAuthor = new List<Message>();

            foreach(var message in messages)
            {
                if (secondList.Where(x => x.message == message.message).Count() != 0)
                {
                    duplicateMessage.Add(message);
                }
                else if (secondList.Where(x => x.author == message.author).Count() != 0)
                {
                    duplicateAuthor.Add(message);
                }
                else
                {
                    secondList.Add(message);
                }
            }
        }
    }

    struct Message
    {
        public float width { get; set; }
        public float height { get; set; }
        public string message { get; set; }
        public string author { get; set; }
    }
}
