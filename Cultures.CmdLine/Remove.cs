using System;
using System.Globalization;
using System.Linq;
using CommandLine;

namespace Cultures.CmdLine
{
    [Verb("remove", HelpText = "Removes an installed custom culture")]
    class Remove
    {
        [Option('c', "culture", Required = true, HelpText = "Culture to remove")]
        public string Culture { get; set; }

        public int Action()
        {
            var culture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(x => x.Name.ToLowerInvariant() == Culture.ToLowerInvariant());
            if (culture != null)
            {
                try
                {
                    CultureAndRegionInfoBuilder.Unregister(culture.Name);
                    Console.WriteLine($"Removed '{culture.Name}'.");
                    return 0;
                }
                catch (ArgumentException)
                {
                    Console.WriteLine("Could not remove this culture - are you sure this a custom culture?");
                }
            }
            else
            {
                Console.WriteLine($"Culture '{Culture}' not found.");
            }
            return 1;
        }
    }
}