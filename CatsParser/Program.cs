using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Drawing;

namespace CatsParser
{
    class Program
    {
        const string NameLineRegex = @"\b(Name: [\w ]*)\b";
        const string CatLineRegex = @"\b(Cat[0-9]?: [\w ]*)\b";
        const string CatsKeyRegex = @"\b(Cat[0-9]?:[ ]?)\b";
        const string MessageKey = "Message:";
        const string ProcessedImagesFolder = "images-processed";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Loading raws...");
            var rawLines = await System.IO.File.ReadAllLinesAsync("catsraw.txt");

            Console.WriteLine("Loading Image information...");
            var imagesOriginal = System.IO.Directory.GetFiles("images");

            Console.WriteLine("Processing Images...");
            System.IO.Directory.CreateDirectory(ProcessedImagesFolder);
            var processingTasks = new List<Task>();
            foreach(var image in imagesOriginal)
            {
                var fileName = $"{ProcessedImagesFolder}/{image.Split("/").Last().Split(".").First()}";

                // - Creating webp
                var webpProcess = new System.Diagnostics.Process();
                webpProcess.StartInfo.FileName = "cwebp";
                webpProcess.StartInfo.Arguments = $"{image} -o {fileName}.webp";
                webpProcess.Start();

                // - Creating jpg
                var jpgProcess = new System.Diagnostics.Process();
                jpgProcess.StartInfo.FileName = "convert";
                jpgProcess.StartInfo.Arguments = $"{image} {fileName}.jpg";
                jpgProcess.Start();

                processingTasks.Add(webpProcess.WaitForExitAsync());
                processingTasks.Add(jpgProcess.WaitForExitAsync());
            }

            // - Waiting for all image convertions to complete
            Task.WaitAll(processingTasks.ToArray());

            Console.WriteLine("Processing document...");
            var imageFileNames = imagesOriginal.Select(image => image.Split("/").Last().Split(".").First()).ToList();
            var cats = new List<Cat>();
            bool isMessage = false;
            foreach(var line in rawLines)
            {
                // - Skipping empty lines
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                // - Parsing author name
                if (Regex.IsMatch(line, NameLineRegex))
                {
                    // - Cleaning up previous cat data
                    isMessage = false;
                    if (cats.Count > 0)
                    {
                        var previousCat = cats[cats.Count - 1];
                        cats[cats.Count - 1].Message = previousCat.Message.Trim();

                        var otherImages = imageFileNames
                            .Where(n => 
                                n.Contains(previousCat.Artist.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase) ||
                                n.Contains(previousCat.Artist.Replace(" ", "_"), StringComparison.CurrentCultureIgnoreCase))
                            .ToList();
                        
                        for (int iter = 1; iter < otherImages.Count; iter++)
                        {
                            var remainingImageData = Image.FromFile($"{ProcessedImagesFolder}/{otherImages[iter]}.jpg");
                            cats.Add(new Cat{
                                Artist = previousCat.Artist,
                                Width = 1,
                                Height = (double)remainingImageData.Height / (double)remainingImageData.Width,
                                Source = $"/birthday21/cats/{otherImages[iter]}",
                                Cats = previousCat.Cats,
                                Message = ""
                            });
                        }
                    }

                    // - Adding new cat data
                    var name = line.Remove(0, 5).Trim();
                    var fileName = imageFileNames
                        .Where(n => 
                            n.Contains(name.Replace(" ", ""), StringComparison.CurrentCultureIgnoreCase) ||
                            n.Contains(name.Replace(" ", "_"), StringComparison.CurrentCultureIgnoreCase))
                        .FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        throw new Exception("File Missing");
                    }
                    var imageData = Image.FromFile($"{ProcessedImagesFolder}/{fileName}.jpg");
                    cats.Add(new Cat{
                        Artist = name,
                        Width = 1,
                        Height = (double)imageData.Height / (double)imageData.Width,
                        Source = $"/birthday21/cats/{fileName}",
                        Cats = new List<string>(),
                        Message = ""
                    });
                }
                // - Parsing cat name
                else if (Regex.IsMatch(line, CatLineRegex))
                {
                    var name = line.Remove(0, 5).Trim();
                    cats[cats.Count - 1].Cats.Add(Regex.Replace(line, CatsKeyRegex, "").Trim());
                }
                // - Parsing start of message
                else if (line.StartsWith(MessageKey))
                {
                    cats[cats.Count - 1].Message = line.Replace(MessageKey, "").Trim();
                    isMessage = true;
                }
                // - Parse message
                else if (isMessage)
                {
                    cats[cats.Count - 1].Message += $" {line}";
                }
            }
            Console.WriteLine("Generating final document...");
            System.IO.File.WriteAllText("catData.json", JsonConvert.SerializeObject(cats));
        }
    }

    public class Cat {

        [JsonProperty("src")]
        public string Source { get; set; }

        [JsonProperty("width")]
        public double Width { get; set; }
        
        [JsonProperty("height")]
        public double Height { get; set; }

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