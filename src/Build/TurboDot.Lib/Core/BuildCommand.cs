using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Nito.AsyncEx;
using System.Threading.Tasks;
using TurboBase.IO;
using TurboCompile.API;
using TurboCompile.CSharp;
using TurboCompile.VBasic;
using TurboDot.Tools;
using TurboMeta.API.Proj;
using TurboRepo.API;
using TurboRepo.API.External;
using TurboRepo.Nuget;
using static TurboDot.Tools.Defaults;
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
            var files = DotCli.GetSlnOrProject(result);
            if (files == null)
            {
                DotCli.ShowSlnOrProjectError();
                return;
            }

            var loader = DotUtil.CreateLoader();
            var projects = files.SelectMany(f =>
                DotCli.ReadSlnOrProject(loader, f).ProjectsInOrder).ToArray();

            var g = new NuGet();

            var noRestore = result.GetValueForOption(NoRestoreOption);
            if (!noRestore)
            {
                await Restore(projects, g);
                LogSink.Write(" Done with restoring.");
            }

            await Build(projects, g);
            LogSink.Write(" Done with building.");
        }

        private static void DoCompile(IExtRefResolver extResolver, IProject handle)
        {
            var abs = handle.FilePath;
            LogSink.Write(@$"  Building project ""{abs}""...");
            var projDir = handle.GetFolder();
            var binDir = GetBinFolder(projDir);

            var projName = handle.Name;
            var projBinDll = Path.Combine(binDir, $"{projName}.dll");
            var projBinInfo = new FileInfo(projBinDll);
            
            var (compiler, paths) = ListFiles(projDir, handle);
            if (projBinInfo.Exists)
            {
                var lastWriteDll = projBinInfo.LastWriteTime;
                var lastWriteSrc = paths.Select(File.GetLastWriteTime).Max();
                if (lastWriteSrc <= lastWriteDll)
                    return;
            }

            var packs = handle.PackageReferences
                .Select(p => (IExternalRef)new NuGetRef(p.Name, p.Version));
            var projs = handle.ProjectReferences
                .Select(p => new LocalRef(handle.GetFullPath(p)));
            var locals = handle.LocalReferences
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

            var kind = handle.OutputMode.ToKind();
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
            string dir, IProject project)
        {
            var lang = Path.GetExtension(project.FilePath)?.TrimStart('.');
            ICompiler compiler;
            string[] files;
            if (lang is "vbproj" or "vb")
            {
                compiler = new VBasicCompiler();
                files = project.IncludedItems.ToArray();
            }
            else if (lang is "csproj" or "cs")
            {
                compiler = new CSharpCompiler();
                files = project.IncludedItems.ToArray();
            }
            else
            {
                throw new InvalidOperationException($"{lang} ?!");
            }
            var binPart = IoTools.GetPathPart("bin");
            if (!dir.Contains(binPart))
                files = files.Where(f => !f.Contains(binPart)).ToArray();
            return (compiler, files);
        }

        private static async Task Build(ICollection<IProject> projects, NuGet nuGet)
        {
            var resolver = new NuGetResolver(allowDownload: false, nuGet: nuGet);

            var events = projects.ToDictionary(
                k => k.FilePath,
                _ => new AsyncManualResetEvent(false));

            var tasks = projects.Select(f => Task.Run(async () =>
            {
                foreach (var projRef in f.ProjectReferences)
                    await events[f.GetFullPath(projRef)].WaitAsync();
                DoCompile(resolver, f);
                events[f.FilePath].Set();
            }));
            await Task.WhenAll(tasks);

            events.Clear();
        }
    }
}