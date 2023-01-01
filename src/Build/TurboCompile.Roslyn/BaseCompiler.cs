using TurboCompile.API;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using TurboCompile.Common;
using MR = Microsoft.CodeAnalysis.MetadataReference;

namespace TurboCompile.Roslyn
{
    public abstract class BaseCompiler : ICompiler
    {
        protected readonly AssemblyCache<MR> AssemblyCache = MetaTool.CreateCache();

        public CompileResult Compile(CompileArgs args)
        {
            var first = args.Paths.FirstOrDefault();
            var extra = GetExtraCode(Path.GetFileNameWithoutExtension(first));
            var sources = new List<(string, string)> { (nameof(extra), extra) };
            var enc = Encoding.UTF8;
            sources.AddRange(args.Paths.Select(path =>
                (Path.GetFullPath(path), File.ReadAllText(path, enc))));
            using var memory = new MemoryStream();
            var compile = GenerateCode(args.Debug, sources);
            var result = compile.Emit(memory);
            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
                var fails = new List<(string, string)>();
                foreach (var diagnostic in failures)
                    fails.Add((diagnostic.Id, diagnostic.GetMessage()));
                throw new CompileError(fails);
            }
            memory.Seek(0, SeekOrigin.Begin);
            var rtJson = Globals.Net6RtJson;
            return new CompileResult(memory.ToArray(), rtJson);
        }

        protected abstract Compilation GenerateCode(bool debug, ICollection<(string, string)> sources);

        protected abstract string GetExtraCode(string name);

        protected Assembly GetRuntimeAssembly()
        {
            return Assembly.Load(new AssemblyName("System.Runtime"));
        }
    }
}