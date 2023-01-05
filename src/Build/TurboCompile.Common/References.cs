using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using SingleFileExtractor.Core;

namespace TurboCompile.Common
{
    public static class References
    {
        private static readonly Lazy<string[]> CompileLibs = new(FindRefs);

        private static string[] FindRefs()
        {
            var ctx = DependencyContext.Default;
            var refs = ctx?.CompileLibraries
                .SelectMany(c => c.ResolveReferencePaths())
                .ToArray();
            return refs;
        }

        public static string ReplaceWithRef(this AssemblyName assembly)
        {
            var name = $"{assembly.Name}.dll";
            var found = CompileLibs.Value?
                .FirstOrDefault(l => Path.GetFileName(l) == name);
            return found;
        }

        public static FileEntry ReplaceWithRef(this Manifest manifest, AssemblyName assembly)
        {
            if (manifest == null)
                return null;

            var runName = $"{assembly.Name}.dll";
            var refName = $"refs/{runName}";

            var embedded = manifest.Files.FirstOrDefault(f =>
                f.Type == FileType.Assembly && f.RelativePath == refName);

            if (embedded == null)
                embedded = manifest.Files.FirstOrDefault(f =>
                    f.Type == FileType.Assembly && f.RelativePath == runName);

            return embedded;
        }
    }
}