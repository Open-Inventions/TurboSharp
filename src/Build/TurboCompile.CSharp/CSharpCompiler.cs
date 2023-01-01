using TurboCompile.Roslyn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;

namespace TurboCompile.CSharp
{
    public sealed class CSharpCompiler : BaseCompiler
    {
        public const string Extension = ".cs";

        protected override Compilation GenerateCode(bool debug, ICollection<(string, string)> sources)
        {
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
            var trees = sources.Select(source =>
            {
                var code = SourceText.From(source.Item2);
                return SyntaxFactory.ParseSyntaxTree(code, options, source.Item1);
            });
            var rtAss = GetRuntimeAssembly();
            var references = AssemblyCache.Locate(new[]
            {
                rtAss,
                typeof(object).Assembly,
                typeof(Console).Assembly,
                typeof(CSharpArgumentInfo).Assembly,
                typeof(Queryable).Assembly,
                typeof(HttpClient).Assembly
            });
            var detail = new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                optimizationLevel: debug ? OptimizationLevel.Debug : OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            return CSharpCompilation.Create("Runner", trees,
                references: references, options: detail);
        }

        protected override string GetExtraCode(string name)
        {
            var info = new CsGlobals().SetNameAndVer(name);
            var code = info.Generate();
            return code;
        }
    }
}