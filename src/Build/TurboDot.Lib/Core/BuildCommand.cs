using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Nito.AsyncEx;
using System.Threading.Tasks;
using TurboCompile.API;
using TurboCompile.Common;
using TurboCompile.CSharp;
using TurboCompile.VBasic;
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

        private static void DoCompile(NuGet nuGet, ProjectHandle handle)
        {
            var abs = handle.GetFile();
            LogSink.Write(@$"  Building project ""{abs}""...");
            var projDir = handle.GetFolder();
            var binDir = GetBinFolder(projDir);

            var projName = handle.Meta.ProjectName;
            var projBinDll = Path.Combine(binDir, $"{projName}.dll");
            var projBinInfo = new FileInfo(projBinDll);
            var (compiler, paths) = ListFiles(handle.Lang, projDir);
            if (projBinInfo.Exists)
            {
                var lastWriteDll = projBinInfo.LastWriteTime;
                var lastWriteSrc = paths.Select(File.GetLastWriteTime).Max();
                if (lastWriteSrc <= lastWriteDll)
                    return;
            }

            var meta = new AssemblyMeta(projName);
            var args = new CompileArgs(paths, meta);
            var compiled = compiler.Compile(args);
            File.WriteAllBytes(projBinDll, compiled.RawAssembly);

            var projBinRt = Path.Combine(binDir, $"{projName}.runtimeconfig.json");
            File.WriteAllText(projBinRt, compiled.RuntimeJson);
        }

        private static string GetBinFolder(string dir,
            string framework = "net6.0", string mode = "Debug")
        {
            var folder = Path.Combine(dir, "bin", mode, framework);
            return Directory.CreateDirectory(folder).FullName;
        }

        private static (ICompiler compiler, string[] files) ListFiles(
            ProjectLang lang, string dir)
        {
            const SearchOption o = SearchOption.AllDirectories;
            ICompiler compiler;
            string[] files;
            if (lang == ProjectLang.VbProj)
            {
                compiler = new VBasicCompiler();
                files = Directory.GetFiles(dir, "*.vb", o);
            }
            else
            {
                compiler = new CSharpCompiler();
                files = Directory.GetFiles(dir, "*.cs", o);
            }
            var binPart = IoTools.GetPathPart("bin");
            files = files.Where(f => !f.Contains(binPart)).ToArray();
            return (compiler, files);
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