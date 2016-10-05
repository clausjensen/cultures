using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace Cultures.CmdLine
{
    [Verb("import", HelpText = "Import culture from file")]
    class Import
    {
        private Regex _cultureRegex = new Regex(@"(?:\\)?([A-z]{2}-[A-z]{2})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

        [Option('c', "culture", HelpText = "File to import")]
        public string InputFile { get; set; }

        [Option(HelpText = "Path to files")]
        public string OutputPath { get; set; }

        public int Action()
        {
            var filePath = InputFile;
            if (File.Exists(filePath) == false)
            {
                filePath = Path.Combine(".", filePath);
                if (File.Exists(filePath) == false)
                {
                    Console.WriteLine($"File not found: '{InputFile}'.");
                    return 1;
                }
            }

            try
            {
                var match = _cultureRegex.Match(InputFile);
                if (match.Success)
                {
                    var cultureName = match.Groups[1].Value;
                    
                    if (CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(x => x.Name.ToLowerInvariant() == cultureName.ToLowerInvariant()) != null)
                    {
                        Console.WriteLine($"Cannot import - culture '{cultureName}' already exists.");
                        return 1;
                    }
                    // Build and register a temporary culture with the name of what we want to import.
                    // CreateFromLdml method will fail when trying to load a culture from file if it doesn't already exist.
                    var tempCulture = new CultureAndRegionInfoBuilder(cultureName, CultureAndRegionModifiers.None);
                    tempCulture.LoadDataFromCultureInfo(CultureInfo.CurrentCulture);
                    tempCulture.LoadDataFromRegionInfo(RegionInfo.CurrentRegion);
                    tempCulture.Register();
                    // Now load up the culture we actually want to import
                    var culture = CultureAndRegionInfoBuilder.CreateFromLdml(filePath);
                    // Unregister the temporary culture
                    CultureAndRegionInfoBuilder.Unregister(cultureName);

                    // Register the real culture loaded from file
                    culture.Register();
                    Console.WriteLine($"Culture '{culture.CultureName}' has been installed.");
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return 0;
        }
    }
}
