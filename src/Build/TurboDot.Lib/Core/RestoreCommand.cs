using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using ByteDev.DotNet.Project;
using TurboDot.Impl;
using TurboDot.Tools;

namespace TurboDot.Core
{
    public static class RestoreCommand
    {
        public static async Task Run(InvocationContext obj)
        {
            await Run(obj.ParseResult);
        }

        private static async Task Run(ParseResult result)
        {
            var files = Cli.GetSlnOrProject(result);
            if (files == null)
            {
                Cli.ShowSlnOrProjectError();
                return;
            }
            const string nugetDir = "nuget";
            var g = new NuGet(nugetDir);
            var projects = files.SelectMany(f =>
                Cli.ListProjects(Cli.ReadSlnOrProject(f)));
            await Restore(projects, g);
            LogSink.Write(" Done with restoring.");
        }

        private static async Task DoRestore(PackageReference packRef, NuGet nuGet)
        {
            var name = packRef.Name;
            var ver = packRef.Version;
            LogSink.Write(@$"  Restoring {name} v{ver} ...");
            await nuGet.Download(name, ver);
        }

        public static async Task Restore(IEnumerable<ProjectHandle> projects, NuGet nuGet)
        {
            var packages = projects
                .SelectMany(p => p.Proj.PackageReferences)
                .GroupBy(p => $"{p.Name}|{p.Version}");

            var tasks = packages.Select(f => Task.Run(async () =>
            {
                var packRef = f.First();
                await DoRestore(packRef, nuGet);
            }));

            await Task.WhenAll(tasks);
        }
    }
}