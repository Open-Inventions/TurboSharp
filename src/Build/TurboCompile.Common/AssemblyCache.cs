using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SingleFileExtractor.Core;
using TurboCompile.API;

namespace TurboCompile.Common
{
    public sealed class AssemblyCache<T>
    {
        private readonly IRefLoader<T> _loader;
        private readonly IDictionary<string, T> _assemblies;
        private readonly Manifest _manifest;

        public AssemblyCache(IRefLoader<T> loader)
        {
            _loader = loader;
            _assemblies = new Dictionary<string, T>();
            var entry = Assembly.GetEntryAssembly()?.Location;
            if (!string.IsNullOrWhiteSpace(entry))
                return;
            var exe = Environment.ProcessPath!;
            var reader = new ExecutableReader();
            _manifest = reader.ReadManifest(exe);
        }

        public T[] Locate(Assembly[] assemblies)
        {
            var references = new T[assemblies.Length];
            for (var i = 0; i < assemblies.Length; i++)
            {
                var assembly = assemblies[i];
                var full = assembly.FullName!;
                if (_assemblies.TryGetValue(full, out var found))
                {
                    references[i] = found;
                    continue;
                }
                var loc = assembly.ReplaceWithRef() ?? assembly.Location;
                if (string.IsNullOrWhiteSpace(loc))
                {
                    var embedded = _manifest.ReplaceWithRef(assembly);
                    if (embedded != null)
                    {
                        var tmpFile = Path.GetFullPath("temp.bin");
                        embedded.Extract(tmpFile);
                        var bytes = File.ReadAllBytes(tmpFile);
                        File.Delete(tmpFile);
                        var eRef = _loader.LoadFrom(bytes);
                        references[i] = _assemblies[full] = eRef;
                        continue;
                    }
                    throw new FileNotFoundException(full);
                }
                var @ref = _loader.LoadFrom(loc);
                references[i] = _assemblies[full] = @ref;
            }
            return references;
        }
    }
}