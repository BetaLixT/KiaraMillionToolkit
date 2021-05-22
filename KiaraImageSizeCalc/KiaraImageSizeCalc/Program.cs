using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Drawing;

namespace KiaraImageSizeCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var canvasImagesJson = System.IO.File.ReadAllText("canvasImages.json");
            var canvasImages = JsonConvert.DeserializeObject<List<CanvasImage>>(canvasImagesJson);
            for(int i = 0; i < canvasImages.Count; i++)
            {
                var image = Image.FromFile(canvasImages[i].src.Substring(1));
                canvasImages[i].height = (double)image.Height / (double)image.Width;
            }
            var canvasImagesUpdateJson = JsonConvert.SerializeObject(canvasImages);
            System.IO.File.WriteAllText("canvasImages-updated.json", canvasImagesUpdateJson);
        }
    }

    class CanvasImage
    {
        public string src { get; set; }
        public double width { get; set; }
        public double height { get; set; }
        public double left { get; set; }
        public double top { get; set; }
        public double angle { get; set; }
        public double opacity { get; set; }
        public double scaleX { get; set; }
        public double scaleY { get; set; }
        public string message { get; set; }
        public string artist { get; set; }
    }
}
