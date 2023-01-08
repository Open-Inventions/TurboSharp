using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ionide.ProjInfo.Sln.Construction;
using TurboDot.Impl;
using TurboDot.Tools;
using static TurboDot.Tools.Defaults;
using Parser = TurboDot.Tools.Parser;

namespace TurboDot
{
    public static class Cli
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

        public static (SolutionFile sol, ProjectHandle prj) ReadSlnOrProject(FileInfo file)
        {
            var full = file.FullName;
            SolutionFile sol = null;
            ProjectHandle prj = null;
            if (file.Extension == ".sln")
            {
                sol = SolutionFile.Parse(full);
            }
            else
            {
                prj = ProjectExt.LoadProject(full, default, default);
            }
            return (sol, prj);
        }

        public static IEnumerable<ProjectHandle> ListProjects(
            (SolutionFile sol, ProjectHandle prj) pair)
        {
            if (pair.prj != null)
                yield return pair.prj;

            if (pair.sol != null)
                foreach (var project in pair.sol.LoadProjects())
                    yield return project;
        }
    }
}