using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using Nito.AsyncEx;
using System.Threading.Tasks;
using TurboDot.Impl;
using static TurboDot.Tools.Defaults;
using TurboDot.Tools;
using static TurboDot.Core.RestoreCommand;

namespace TurboDot.Core
{
    public static class BuildCommand
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
                Cli.ListProjects(Cli.ReadSlnOrProject(f))).ToArray();

            var noRestore = result.GetValueForOption(NoRestoreOption);
            if (!noRestore)
            {
                await Restore(projects, g);
                LogSink.Write(" Done with restoring.");
            }

            await Build(projects, g);
            LogSink.Write(" Done with building.");
        }

        private static void DoCompile(NuGet nuGet, ProjectHandle found)
        {
            Console.WriteLine($"  {nuGet} - {found.Meta.ProjectName}");

            // TODO ?!
        }

        private static async Task Build(ICollection<ProjectHandle> projects, NuGet nuGet)
        {
            var events = projects.ToDictionary(
                k => k.Meta.AbsolutePath,
                _ => new AsyncManualResetEvent(false));

            var tasks = projects.Select(f => Task.Run(async () =>
            {
                foreach (var projRef in f.Proj.ProjectReferences)
                    await events[f.Meta.GetFullPath(projRef)].WaitAsync();
                DoCompile(nuGet, f);
                events[f.Meta.AbsolutePath].Set();
            }));
            await Task.WhenAll(tasks);

            events.Clear();
        }
    }
}