using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommandLine;

namespace Cultures.CmdLine
{
    [Verb("importfolder", HelpText = "Import cultures from files in a folder")]
    class ImportFolder
    {
        private Regex _cultureRegex = new Regex(@"(?:\\)?([A-z]{2}-[A-z]{2})", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        private const string Extension = ".culture";

        [Value(0, HelpText = "Path to files")]
        public string Folder { get; set; }

        public int Action()
        {
            var dir = new DirectoryInfo(Folder);
            if (dir.Exists == false)
            {
                Console.WriteLine($"Invalid folder: '{Folder}'.");
                return 1;
            }
            var files = dir.EnumerateFiles().Where(x => x.Extension == Extension);
            if (!files.Any())
            {
                Console.WriteLine("No culture files found in folder.");
                return 1;
            }
            Console.WriteLine($"Found {files.Count()} culture files.\n");
            var installCount = 0;
            var skipCount = 0;
            foreach (var file in files)
            {
                try
                {
                    var filePath = file.FullName;
                    var match = _cultureRegex.Match(filePath);
                    if (match.Success)
                    {
                        var cultureName = match.Groups[1].Value;

                        if (CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(x => x.Name.ToLowerInvariant() == cultureName.ToLowerInvariant()) != null)
                        {
                            Console.WriteLine($"Skipping '{cultureName}' - culture already exists.");
                            skipCount++;
                            continue;
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
                        installCount++;
                    }
                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            Console.WriteLine($"\nInstalled {installCount} and skipped {skipCount} culture files.");
            return 0;
        }
    }
}
