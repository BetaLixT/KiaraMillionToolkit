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

            var ProcessedImagesFolder = "images";
            foreach (var message in messages)
            {
                if (!string.IsNullOrEmpty(message.src))
                {
                    var imageName = message.src.Split("/").Last().Split(".").First();
                    if(!(System.IO.File.Exists($"{ProcessedImagesFolder}/{imageName}.jpg") && System.IO.File.Exists($"{ProcessedImagesFolder}/{imageName}.webp")))
                    {
                        var rawImage = System.IO.Directory.GetFiles("images-raw/", $"{imageName}.*").FirstOrDefault();
                        if(!string.IsNullOrEmpty(rawImage))
                        {
                            Console.WriteLine(rawImage);
                            var fileName = $"{ProcessedImagesFolder}/{imageName}";

                            // - Creating webp
                            var webpProcess = new System.Diagnostics.Process();
                            webpProcess.StartInfo.FileName = "cwebp";
                            webpProcess.StartInfo.Arguments = $"{rawImage} -o {fileName}.webp";
                            webpProcess.Start();
                            webpProcess.WaitForExit();

                            // - Creating jpg
                            var jpgProcess = new System.Diagnostics.Process();
                            jpgProcess.StartInfo.FileName = "convert";
                            jpgProcess.StartInfo.Arguments = $"{rawImage} {fileName}.jpg";
                            jpgProcess.Start();
                            jpgProcess.WaitForExit();

                            var remainingImageData = Image.FromFile($"{fileName}.jpg");
                            message.height = (float)remainingImageData.Height / (float)remainingImageData.Width;
                        }
                    }
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
