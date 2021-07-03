using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TwCreditParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var rawCredits = JsonConvert.DeserializeObject<List<RawCredit>>(System.IO.File.ReadAllText("raw.json"));
            var credits = new List<SectionCreditDetailed>();

            foreach (var rawCredit in rawCredits)
            {
                var credit = credits.Where(x => x.title == rawCredit.SubSection).FirstOrDefault();
                var index = -1;
                if (credit == null)
                {
                    index = credits.Count;
                    credits.Add(new SectionCreditDetailed{
                        title = rawCredit.SubSection,
                        creditArray = new List<Credit>()
                    });
                }
                else
                {
                    index = credits.Count - 1;
                }


                var roles = new List<string>();
                if (!string.IsNullOrWhiteSpace(rawCredit.EnglishName))
                {
                    roles.Add($"({rawCredit.EnglishName})");
                }
                roles.AddRange(rawCredit.Position.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList());
                credits[index].creditArray.Add(new Credit{name = rawCredit.KanjiName, roles = roles});
            }

            System.IO.File.WriteAllText("credits.json", JsonConvert.SerializeObject(credits));
        }
    }

    public class RawCredit
    {
        public string KanjiName { get; set; }
        public string Position  { get; set; }
        public string EnglishName  { get; set; }
        public string SubSection  { get; set; }
    }

    public class Credit
    {
        public string name { get; set; }
        public List<string> roles { get; set; }
    }

    public class SectionCreditDetailed
    {
        public string title { get; set; }
        public List<Credit> creditArray { get; set; }
    }
}
