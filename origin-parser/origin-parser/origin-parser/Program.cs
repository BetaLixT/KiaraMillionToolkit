using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace originparser
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var lines = System.IO.File.ReadAllLines("originstories.md");

            var originStories = new List<OriginStory>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                else if (line.StartsWith("# "))
                {
                    originStories.Add(new OriginStory {
                        name = line.Replace("#", "").Trim(),
                        message = new List<string>()
                    });
                }
                else
                {
                    originStories.Last().message.Add(line);
                }
            }

            var serialized = JsonConvert.SerializeObject(originStories);
            System.IO.File.WriteAllText("memberOrigins.json", serialized);
        }
    }

    struct OriginStory
    {
        public string name { get; set; }
        public List<string> message { get; set; }
    }
}
