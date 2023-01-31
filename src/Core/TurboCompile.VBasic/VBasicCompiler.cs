using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.VisualBasic;
using TurboCompile.API;
using TurboCompile.Roslyn;
using System;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualBasic;
using TurboRepo.API.External;

namespace TurboCompile.VBasic
{
    public sealed class VBasicCompiler : BaseCompiler<VisualBasicParseOptions,
        VisualBasicCompilationOptions, VisualBasicCompilation>
    {
        protected override AssemblyRef[] GetMinimalRefs()
        {
            return new AssemblyRef[]
            {
                GetRuntimeAssembly(),
                typeof(Console).Assembly,
                typeof(Constants).Assembly,
                typeof(Queryable).Assembly,
                typeof(HttpClient).Assembly
            };
        }

        protected override SyntaxTree Parse((string file, string text) source,
            VisualBasicParseOptions options)
        {
            var code = ReadSource(source.text);
            return SyntaxFactory.ParseSyntaxTree(code, options, source.file);
        }

        protected override string GetExtraCode(AssemblyMeta meta)
        {
            var info = new VbGlobals().SetNameAndVer(meta);
            var code = info.Generate();
            return code;
        }

        protected override VisualBasicParseOptions CreateParseOpts()
        {
            const LanguageVersion langVer = LanguageVersion.VisualBasic16_9;
            var defOpts = VisualBasicParseOptions.Default;
            var options = defOpts.WithLanguageVersion(langVer);
            return options;
        }

        protected override VisualBasicCompilationOptions CreateCompileOpts(CompileArgs args)
        {
            var kind = args.Kind.ToKind();
            var debug = args.Debug;

            var detail = new VisualBasicCompilationOptions(kind,
                optimizationLevel: debug ? OptimizationLevel.Debug : OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            return detail;
        }

        protected override VisualBasicCompilation CreateCompilation(string name,
            IEnumerable<SyntaxTree> trees, IEnumerable<MetadataReference> references,
            VisualBasicCompilationOptions detail)
        {
            var result = VisualBasicCompilation.Create(name, trees,
                references: references, options: detail);
            return result;
        }
    }
}