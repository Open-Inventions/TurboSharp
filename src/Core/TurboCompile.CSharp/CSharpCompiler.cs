using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TurboCompile.API;
using TurboCompile.Roslyn;
using System;
using System.Linq;
using System.Net.Http;
using Microsoft.CSharp.RuntimeBinder;
using TurboRepo.API.External;

namespace TurboCompile.CSharp
{
    public sealed class CSharpCompiler : BaseCompiler<CSharpParseOptions,
        CSharpCompilationOptions, CSharpCompilation>
    {
        protected override AssemblyRef[] GetMinimalRefs()
        {
            return new AssemblyRef[]
            {
                GetRuntimeAssembly(),
                typeof(Console).Assembly,
                typeof(CSharpArgumentInfo).Assembly,
                typeof(Queryable).Assembly,
                typeof(HttpClient).Assembly
            };
        }

        protected override SyntaxTree Parse((string file, string text) source,
            CSharpParseOptions options)
        {
            var code = ReadSource(source.text);
            return SyntaxFactory.ParseSyntaxTree(code, options, source.file);
        }

        protected override string GetExtraCode(AssemblyMeta meta)
        {
            var info = new CsGlobals().SetNameAndVer(meta);
            var code = info.Generate();
            return code;
        }

        protected override CSharpParseOptions CreateParseOpts()
        {
            const LanguageVersion langVer = LanguageVersion.CSharp10;
            var defOpts = CSharpParseOptions.Default;
            var options = defOpts.WithLanguageVersion(langVer);
            return options;
        }

        protected override CSharpCompilationOptions CreateCompileOpts(CompileArgs args)
        {
            var kind = args.Kind.ToKind();
            var debug = args.Debug;

            var detail = new CSharpCompilationOptions(kind,
                optimizationLevel: debug ? OptimizationLevel.Debug : OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            return detail;
        }

        protected override CSharpCompilation CreateCompilation(string name,
            IEnumerable<SyntaxTree> trees, IEnumerable<MetadataReference> references,
            CSharpCompilationOptions detail)
        {
            var result = CSharpCompilation.Create(name, trees,
                references: references, options: detail);
            return result;
        }
    }
}