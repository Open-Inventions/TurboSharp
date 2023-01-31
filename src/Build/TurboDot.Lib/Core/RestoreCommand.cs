using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Threading.Tasks;
using TurboDot.Tools;
using TurboMeta.API.Proj;
using TurboMeta.API.Ref;
using TurboRepo.Nuget;

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
            var files = DotCli.GetSlnOrProject(result);
            if (files == null)
            {
                DotCli.ShowSlnOrProjectError();
                return;
            }

            var g = new NuGet();

            var loader = DotUtil.CreateLoader();
            var projects = files.SelectMany(f =>
                DotCli.ReadSlnOrProject(loader, f).ProjectsInOrder);

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

        public static async Task Restore(IEnumerable<IProject> projects, NuGet nuGet)
        {
            var packages = projects
                .SelectMany(p => p.PackageReferences)
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