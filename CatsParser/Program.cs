using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;

namespace CatsParser
{
    class Program
    {
        const string NameLineRegex = @"\b(Name: [\w ]*)\b";
        const string CatLineRegex = @"\b(Cat[0-9]?: [\w ]*)\b";
        const string CatsKey = @"\b(Cat[0-9]?:[ ]?)\b";
        const string MessageKey = @"\b(Message:[ ]?)\b";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Loading raws...");
            var rawLines = await System.IO.File.ReadAllLinesAsync("catsraw.txt");
            var cats = new List<Cat>();
            bool isMessage = false;
            foreach(var line in rawLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (Regex.IsMatch(line, NameLineRegex))
                {
                    isMessage = false;
                    if (cats.Count > 0)
                    {
                        cats[cats.Count - 1].Message = cats[cats.Count - 1].Message.Trim();
                    }
                    
                    var name = line.Remove(0, 5).Trim();
                    cats.Add(new Cat{
                        Artist = name,
                        Width = 1,
                        Height = 1,
                        Source = $"/birthday21/cats/{name.Replace(" ", "")}_1.png",
                        Cats = new List<string>()
                    });
                }
                else if (Regex.IsMatch(line, CatLineRegex))
                {
                    var name = line.Remove(0, 5).Trim();
                    cats[cats.Count - 1].Cats.Add(Regex.Replace(line, CatsKey, "").Trim());
                }
                else if (Regex.IsMatch(line, MessageKey))
                {
                    cats[cats.Count - 1].Message = Regex.Replace(line, MessageKey, "").Trim();
                    isMessage = true;
                }
                else if (isMessage)
                {
                    cats[cats.Count - 1].Message += $" {line}";
                }
            }
        }
    }

    public class Cat {

        [JsonProperty("src")]
        public string Source { get; set; }

        [JsonProperty("width")]
        public float Width { get; set; }
        
        [JsonProperty("height")]
        public float Height { get; set; }

        [JsonProperty("cats")]
        public List<string> Cats { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("artist")]
        public string Artist { get; set; }

    }
}

/*
    {
      "src": "/birthday21/cats/RayLemon_1.jpg",
      "width": 1.0,
      "height": 0.75,
      "cats": [
        "Pudding"
      ],
      "message": "Happy Birthday tenchou! Every day is full of joy with ur stream! Hopefully we'll learn and grow together to be a better person in many more years :3! Stay healthy and happy <3",
      "artist": "Ray Lemon"
    }
*/