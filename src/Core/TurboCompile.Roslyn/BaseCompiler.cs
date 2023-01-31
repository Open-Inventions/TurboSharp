using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TurboCompile.API;
using TurboCompile.Common;
using TurboRepo.API.External;
using TurboRepo.Nuget;
using System.IO;
using static TurboCompile.Common.Internals;
using MR = Microsoft.CodeAnalysis.MetadataReference;

namespace TurboCompile.Roslyn
{
    public abstract class BaseCompiler<TP, TC, TB> : ICompiler
        where TP : ParseOptions
        where TC : CompilationOptions
        where TB : Compilation
    {
        private readonly AssemblyCache<MR> _cache = OptionTool.CreateCache();

        public CompileResult Compile(CompileArgs raw)
        {
            var args = raw with
            {
                Meta = raw.Meta ?? new AssemblyMeta(
                    Path.GetFileNameWithoutExtension(raw.Paths.FirstOrDefault())
                )
            };
            var extra = GetExtraCode(args.Meta);
            var sources = new List<(string, string)> { (nameof(extra), extra) };
            foreach (var path in args.Paths)
            {
                foreach (var item in ReadCode(path))
                {
                    sources.Add(item);
                }
            }
            using var memory = new MemoryStream();
            var compile = GenerateCode(args, sources);
            var result = compile.Emit(memory);
            if (!result.Success)
            {
                var failures = result.Diagnostics.Where(d =>
                    d.IsWarningAsError || d.Severity == DiagnosticSeverity.Error);
                var fails = new List<(string, string)>();
                foreach (var diagnostic in failures)
                    fails.Add((diagnostic.Id, diagnostic.GetMessage()));
                throw new CompileError(fails);
            }
            memory.Seek(0, SeekOrigin.Begin);
            var rtJson = Globals.Net6RtJson;
            return new CompileResult(memory.ToArray(), rtJson);
        }

        protected Compilation GenerateCode(CompileArgs args, ICollection<(string, string)> sources)
        {
            var name = args.Meta.Name;
            var paOpt = CreateParseOpts();
            var externals = BaseRefs.CreateStdSet();
            if (args.Additional != null)
                Array.ForEach(args.Additional, a => externals.Add(a));

            var trees = sources.Select(s => Parse(s, paOpt)).ToArray();
            var libs = GetMinimalRefs().Concat(externals).ToArray();

            var references = _cache.Locate(libs, args.Resolver);
            var cmOpt = CreateCompileOpts(args);
            return CreateCompilation(name, trees, references, cmOpt);
        }

        protected abstract AssemblyRef[] GetMinimalRefs();

        protected abstract SyntaxTree Parse((string file, string text) source, TP opts);

        protected SourceText ReadSource(string text) => SourceText.From(text);

        protected Assembly GetRuntimeAssembly() => Externals.LoadByName("System.Runtime");

        protected abstract string GetExtraCode(AssemblyMeta meta);

        protected abstract TP CreateParseOpts();

        protected abstract TC CreateCompileOpts(CompileArgs args);

        protected abstract TB CreateCompilation(string name,
            IEnumerable<SyntaxTree> trees, IEnumerable<MR> references, TC detail);
    }
}