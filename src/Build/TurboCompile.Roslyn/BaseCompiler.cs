﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using TurboCompile.API;
using TurboCompile.Common;
using MR = Microsoft.CodeAnalysis.MetadataReference;

namespace TurboCompile.Roslyn
{
    public abstract class BaseCompiler : ICompiler
    {
        protected readonly AssemblyCache<MR> AssemblyCache = MetaTool.CreateCache();

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
            var enc = Encoding.UTF8;
            sources.AddRange(args.Paths.Select(path =>
                (Path.GetFullPath(path), File.ReadAllText(path, enc))));
            using var memory = new MemoryStream();
            var compile = GenerateCode(args, sources);
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

        protected abstract Compilation GenerateCode(CompileArgs args, ICollection<(string, string)> sources);

        protected abstract string GetExtraCode(AssemblyMeta meta);

        protected Assembly GetRuntimeAssembly()
        {
            return Assembly.Load(new AssemblyName("System.Runtime"));
        }
    }
}