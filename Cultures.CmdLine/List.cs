using System;
using System.Globalization;
using System.Linq;
using CommandLine;

namespace Cultures.CmdLine
{
    [Verb("list", HelpText = "Lists cultures installed on this machine")]
    class List
    {
        [Option('s', "startswith", HelpText = "Starts with (example: en)")]
        public string StartsWith { get; set; }

        public int Action()
        {
            var message = "\nListing all cultures installed on this machine:";
            var c = CultureInfo.GetCultures(CultureTypes.AllCultures);
            if (string.IsNullOrWhiteSpace(StartsWith) == false)
            {
                c = c.Where(x => x.Name.ToLowerInvariant().StartsWith(StartsWith.ToLowerInvariant())).ToArray();
                if (c.Length == 0)
                    message = $"No cultures found starting with: '{StartsWith}'";
                else
                    message = $"Listing {c.Length} cultures starting with: '{StartsWith}'";
            }
            Console.WriteLine(message);
            foreach (var cultureInfo in c.Where(x => x.Name != string.Empty).Select((e, i) => new { Item = e, Grouping = (i / 5) }).GroupBy(e => e.Grouping))
            {
                Console.Write("\n");
                foreach (var cc in cultureInfo)
                {
                    Console.Write(cc.Item.Name.PadRight(20));
                }
            }
            return 0;
        }
    }
}