﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic;
using TurboCompile.API;
using TurboCompile.Roslyn;

namespace TurboCompile.VBasic
{
    public sealed class VBasicCompiler : BaseCompiler
    {
        public const string Extension = ".vb";

        protected override Compilation GenerateCode(CompileArgs args, ICollection<(string, string)> sources)
        {
            var name = args.Meta.Name;
            var debug = args.Debug;

            var options = VisualBasicParseOptions.Default.WithLanguageVersion(LanguageVersion.VisualBasic16_9);
            var trees = sources.Select(source =>
            {
                var code = SourceText.From(source.Item2);
                return SyntaxFactory.ParseSyntaxTree(code, options, source.Item1);
            });
            var rtAss = GetRuntimeAssembly();
            var references = AssemblyCache.Locate(new[]
            {
                rtAss,
                typeof(Console).Assembly,
                typeof(Constants).Assembly,
                typeof(Queryable).Assembly,
                typeof(HttpClient).Assembly
            });
            var detail = new VisualBasicCompilationOptions(OutputKind.ConsoleApplication,
                optimizationLevel: debug ? OptimizationLevel.Debug : OptimizationLevel.Release,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            return VisualBasicCompilation.Create(name, trees,
                references: references, options: detail);
        }

        protected override string GetExtraCode(AssemblyMeta meta)
        {
            var info = new VbGlobals().SetNameAndVer(meta);
            var code = info.Generate();
            return code;
        }
    }
}