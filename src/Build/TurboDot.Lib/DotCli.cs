using System.Threading.Tasks;
using TurboDot.Tools;
using TurboMeta.API.Sol;
using System.CommandLine;
using Parser = TurboDot.Tools.Parser;
using System.CommandLine.Parsing;
using System.IO;
using System;
using System.Linq;
using static TurboDot.Tools.Defaults;
using TurboMeta.API.File;

namespace TurboDot
{
    public static class DotCli
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = Parser.Build(Parser.RootCommand);
            return await rootCommand.InvokeAsync(args);
        }

        public static FileInfo[] GetSlnOrProject(ParseResult result, string root = null)
        {
            var infos = result.GetValueForArgument(SlnOrProjectArgument)
                .Select(i => new FileInfo(i))
                .ToArray();
            if (infos.Length >= 1)
            {
                return infos;
            }

            const SearchOption o = SearchOption.TopDirectoryOnly;
            root ??= Environment.CurrentDirectory;

            var sol = Directory.GetFiles(root, "*.sln", o);
            if (sol.Length == 1)
            {
                return new[] { new FileInfo(sol[0]) };
            }

            var prj = Directory.GetFiles(root, "*.??proj", o);
            if (prj.Length == 1)
            {
                return new[] { new FileInfo(prj[0]) };
            }

            return null;
        }

        public static void ShowSlnOrProjectError()
        {
            LogSink.ShowError("Specify a project or solution file. The current working" +
                              " directory does not contain a project or solution file.");
            Environment.ExitCode = -1;
        }

        public static ISolution ReadSlnOrProject(MultiFileLoader loader, FileInfo file)
        {
            var full = file.FullName;
            var loaded = loader.Load(full);
            return loaded;
        }
    }
}