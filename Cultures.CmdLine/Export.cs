using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CommandLine;

namespace Cultures.CmdLine
{
    [Verb("export", HelpText = "Export culture to file")]
    class Export
    {
        [Option('c', "culture", Required =  true)]
        public IEnumerable<string> Culture { get; set; }
        
        [Option('o', "output", HelpText = "Path to export files to")]
        public string Output { get; set; }

        public int Action()
        {
            if (string.IsNullOrWhiteSpace(Output))
                Output = ".";

            CultureAndRegionInfoBuilder culture;
            foreach (var cultureName in Culture)
            {
                var c = CultureInfo.GetCultureInfo(cultureName);
                var cultureTypes = c.CultureTypes;
                if (cultureTypes.HasFlag(CultureTypes.UserCustomCulture))
                    culture = new CultureAndRegionInfoBuilder(cultureName, CultureAndRegionModifiers.None);
                else
                    culture = new CultureAndRegionInfoBuilder(cultureName, CultureAndRegionModifiers.Replacement);

                culture.LoadDataFromCultureInfo(new CultureInfo(cultureName));
                culture.LoadDataFromRegionInfo(new RegionInfo(cultureName.Split(Convert.ToChar("-"))[1]));

                var pathToFile = Path.Combine(Output, culture.CultureName + ".culture");

                Directory.CreateDirectory(Output);

                if (File.Exists(pathToFile))
                {
                    File.Delete(pathToFile);
                }
                culture.Save(pathToFile);
                Console.WriteLine($"Exported '{culture.CultureName}' to '{pathToFile}'.");
            }
            return 0;
        }

        //[Option('e', "export", Required = true, HelpText = "Input files to be processed.")]
        //public IEnumerable<string> InputFiles { get; set; }

        // Omitting long name, default --verbose
        //[Option(HelpText = "Prints all messages to standard output.")]
        //public bool Verbose { get; set; }

        //[Option(Default = "中文", HelpText = "Content language.")]
        //public string Language { get; set; }

        //[Value(0, MetaName = "offset", HelpText = "File offset.")]
        //public long? Offset { get; set; }
    }
}