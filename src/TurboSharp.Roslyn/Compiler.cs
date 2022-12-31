using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CSharp.RuntimeBinder;
using TurboSharp.Roslyn.Core;
using static TurboSharp.Roslyn.Core.Globals;

namespace TurboSharp.Roslyn
{
    public static class Compiler
    {
        public static (byte[] b, string j) Compile(string path, bool debug = true)
        {
            var code = File.ReadAllText(path, Encoding.UTF8);
            using var memory = new MemoryStream();
            var compile = GenerateCode(debug, Net6Usings, code);
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
            var rtJson = Net6RtJson;
            return (memory.ToArray(), rtJson);
        }

        private static CSharpCompilation GenerateCode(bool debug, params string[] sources)
        {
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp10);
            var trees = sources.Select(source =>
            {
                var code = SourceText.From(source);
                return SyntaxFactory.ParseSyntaxTree(code, options);
            });
            var rtAss = Assembly.Load(new AssemblyName("System.Runtime"));
            var references = Assemblies.Locate(new[]
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
            return CSharpCompilation.Create(nameof(Runner), trees,
                references: references, options: detail);
        }
    }
}