using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;
using TurboCompile.API;
using TurboCompile.API.External;
using TurboCompile.Common;
using TurboCompile.Roslyn;

namespace TurboCompile.CSharp
{
    public sealed class CSharpCompiler : BaseCompiler
    {
        public const string Extension = ".cs";

        protected override Compilation GenerateCode(CompileArgs args, ICollection<(string, string)> sources)
        {
            var name = args.Meta.Name;
            var debug = args.Debug;

            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
            var externals = new HashSet<IExternalRef>();
            var trees = sources.Select(source =>
            {
                var code = ReadSource(source.Item2, externals);
                return SyntaxFactory.ParseSyntaxTree(code, options, source.Item1);
            }).ToArray();
            var libs = new AssemblyRef[]
                {
                    GetRuntimeAssembly(),
                    typeof(Console).Assembly,
                    typeof(CSharpArgumentInfo).Assembly,
                    typeof(Queryable).Assembly,
                    typeof(HttpClient).Assembly
                }
                .Concat(externals).ToArray();
            var references = AssemblyCache.Locate(libs, args.Resolver);
            var detail = new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                optimizationLevel: debug ? OptimizationLevel.Debug : OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            return CSharpCompilation.Create(name, trees,
                references: references, options: detail);
        }

        protected override string GetExtraCode(AssemblyMeta meta)
        {
            var info = new CsGlobals().SetNameAndVer(meta);
            var code = info.Generate();
            return code;
        }

        private static SourceText ReadSource(string text, ISet<IExternalRef> refs)
        {
            var tmp = Externals.Parse("//#r", text);
            Array.ForEach(tmp, r => refs.Add(r));

            var source = SourceText.From(text);
            return source;
        }
    }
}