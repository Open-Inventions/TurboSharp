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
using TurboCompile.API.External;

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

        private static void DoCompile(IExtRefResolver extResolver, ProjectHandle handle)
        {
            var abs = handle.GetFile();
            LogSink.Write(@$"  Building project ""{abs}""...");
            var projDir = handle.GetFolder();
            var binDir = GetBinFolder(projDir);

            var projName = handle.ProjectName;
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

            var packs = handle.Proj.PackageReferences
                .Select(p => (IExternalRef)new NuGetRef(p.Name, p.Version));
            var projs = handle.Proj.ProjectReferences
                .Select(p => new LocalRef(handle.GetFullPath(p)));
            var locals = handle.Proj.LocalReferences
                .Select(p => new LocalRef(handle.GetFullPath(p)));
            var dependencies = packs.Concat(projs).Concat(locals).ToArray();

            var resolver = new FuncExtRefResolver(e =>
            {
                var ePath = extResolver.Locate(e);
                var eName = Path.GetFileNameWithoutExtension(ePath);
                if (ePath.EndsWith("proj"))
                {
                    var depPrj = Path.GetDirectoryName(ePath) ?? string.Empty;
                    var depBin = Path.Combine(depPrj, "bin", "Debug", "net6.0");
                    ePath = Path.Combine(depBin, $"{eName}.dll");
                }
                var depDllName = Path.GetFileName(ePath);
                var depDestPath = Path.Combine(binDir, depDllName);
                File.Copy(ePath, depDestPath, overwrite: true);
                return depDestPath;
            });

            var kind = handle.Proj.OutputType.ToKind();
            var meta = new AssemblyMeta(projName);
            var args = new CompileArgs(paths, kind, meta, resolver, dependencies);
            var compiled = compiler.Compile(args);
            File.WriteAllBytes(projBinDll, compiled.RawAssembly);

            if (kind == OutputType.Library)
                return;

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
            if (!dir.Contains(binPart))
                files = files.Where(f => !f.Contains(binPart)).ToArray();
            return (compiler, files);
        }

        private static async Task Build(ICollection<ProjectHandle> projects, NuGet nuGet)
        {
            var resolver = new NuGetResolver(allowDownload: false, nuGet: nuGet);

            var events = projects.ToDictionary(
                k => k.AbsolutePath,
                _ => new AsyncManualResetEvent(false));

            var tasks = projects.Select(f => Task.Run(async () =>
            {
                foreach (var projRef in f.Proj.ProjectReferences)
                    await events[f.GetFullPath(projRef)].WaitAsync();
                DoCompile(resolver, f);
                events[f.AbsolutePath].Set();
            }));
            await Task.WhenAll(tasks);

            events.Clear();
        }
    }
}