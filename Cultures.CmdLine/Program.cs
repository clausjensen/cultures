using System;
using CommandLine;

namespace Cultures.CmdLine
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine();
            //args = new[] { "remove", "-c", "ru-ua" };
            //args = new[] { "remove", "-c", "ru-zz" };
            //args = new[] { "import", "-c", "c:\\temp\\cult\\ru-zz.culture" };
            //args = new[] { "import", "-c", "ru-zz.culture" };
            //args = new[] { "importfolder", "c:\\temp\\cult" };
            //args = new[] { "exportall", "c:\\temp\\test" };
            //args = new[] { "exportall" };
            var result = Parser.Default.ParseArguments<List, Export, ExportAll, Import, ImportFolder, Remove>(args);
            var exitCode = result
              .MapResult<List, Export, ExportAll, Import, ImportFolder, Remove, object>(
                list => list.Action(),
                export => export.Action(),
                exportAll => exportAll.Action(),
                import => import.Action(),
                importFolder => importFolder.Action(),
                remove => remove.Action(),
                errors =>
                {
                    //LogHelper.Log(errors);
                    return 1;
                });
            Console.WriteLine();
            return (int)exitCode;
        }
    }
}
