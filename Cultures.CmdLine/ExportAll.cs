using System;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;

namespace Cultures.CmdLine
{
    /// <summary>
    /// This will try to export as many as possible of the cultures existing on the machine.
    /// Cultures with no or multiple separators (dashes in the name) will be ignored as I'm not sure they are real cultures or how they should be handled.
    /// Cultures using Custom calendars (whatever that is) will also be ignored (eat the exception).
    /// Some cultures will throw a 'is not supported' exception. Those will also be ignored (eat the exception).
    /// </summary>
    [Verb("exportall", HelpText = "Tries to export all cultures that can be exported to files")]
    class ExportAll
    {
        [Value(0, HelpText = "Path to export files to")]
        public string Output { get; set; }

        public int Action()
        {
            if (string.IsNullOrWhiteSpace(Output))
                Output = ".\\exported";

            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => x.Name.Count(y => y == '-') == 1);
            foreach (var cultureInfo in allCultures)
            {
                if (string.IsNullOrWhiteSpace(cultureInfo.Name))
                    continue;
                var cultureName = cultureInfo.Name;
                var cultureTypes = cultureInfo.CultureTypes;
                CultureAndRegionInfoBuilder culture = null;

                try
                {
                    if (cultureTypes.HasFlag(CultureTypes.UserCustomCulture))
                        culture = new CultureAndRegionInfoBuilder(cultureName, CultureAndRegionModifiers.None);
                    else
                        culture = new CultureAndRegionInfoBuilder(cultureName, CultureAndRegionModifiers.Replacement);
                    
                    culture.LoadDataFromCultureInfo(new CultureInfo(cultureName));
                    culture.LoadDataFromRegionInfo(new RegionInfo(cultureName.Split(Convert.ToChar("-"))[1]));
                }
                catch (Exception e)
                {
                    if (!e.Message.StartsWith("Custom calendars are not currently supported") && !(e.Message.StartsWith("Culture name '") && e.Message.Contains("' is not supported.\r\nParameter name: name")) && !e.Message.StartsWith("The CultureAndRegionModifiers.Neutral flag does not match the CultureInfo.Neutral property for culture"))
                        throw;
                }
                if (culture == null)
                    continue;
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
    }
}