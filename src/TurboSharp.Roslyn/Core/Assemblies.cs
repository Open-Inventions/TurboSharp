// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SingleFileExtractor.Core;
using MR = Microsoft.CodeAnalysis.MetadataReference;

namespace TurboSharp.Roslyn.Core
{
    public static class Assemblies
    {
        private static readonly IDictionary<string, MR> _assemblies;
        private static readonly Manifest _manifest;

        static Assemblies()
        {
            _assemblies = new Dictionary<string, MR>();
            var entry = Assembly.GetEntryAssembly()?.Location;
            if (!string.IsNullOrWhiteSpace(entry))
                return;
            var exe = Environment.ProcessPath!;
            var reader = new ExecutableReader();
            _manifest = reader.ReadManifest(exe);
        }

        public static MR[] Locate(Assembly[] assemblies)
        {
            var references = new MR[assemblies.Length];
            for (var i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var full = assembly.FullName!;
                if (_assemblies.TryGetValue(full, out var found))
                {
                    references[i] = found;
                    continue;
                }
                var loc = assembly.Location;
                if (string.IsNullOrWhiteSpace(loc))
                {
                    var fileName = $"{assembly.GetName().Name}.dll";
                    var embedded = _manifest?.Files.FirstOrDefault(f =>
                        f.Type == FileType.Assembly && f.RelativePath == fileName);
                    if (embedded != null)
                    {
                        var tmpFile = Path.GetFullPath("temp.bin");
                        embedded.Extract(tmpFile);
                        var bytes = File.ReadAllBytes(tmpFile);
                        File.Delete(tmpFile);
                        var eRef = MR.CreateFromImage(bytes);
                        references[i] = _assemblies[full] = eRef;
                        continue;
                    }
                    throw new FileNotFoundException(full);
                }
                var @ref = MR.CreateFromFile(loc);
                references[i] = _assemblies[full] = @ref;
            }
            return references;
        }
    }
}