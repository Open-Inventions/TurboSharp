using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using TurboCompile.API.External;

namespace TurboRun
{
    public sealed class LocalRunner : IRunner
    {
        public bool Execute(byte[] assembly, string[] args,
            Streams streams = null, IExtRefResolver resolver = null,
            string[] searchPaths = null)
        {
            var weakRef = LoadAndExecute(assembly, args, streams, resolver, searchPaths);
            for (var i = 0; i < 8 && weakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return !weakRef.IsAlive;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] assembly, string[] args,
            Streams con, IExtRefResolver resolver, string[] searchPaths)
        {
            using var memory = new MemoryStream(assembly);
            var context = new UnloadableContext();
            var loaded = context.LoadFromStream(memory);
            Load(context, loaded, resolver, searchPaths);
            var entry = loaded.EntryPoint;
            Streams old = null;
            if (con != null)
            {
                old = Streams.Save();
                con.Load();
            }
            _ = entry != null && entry.GetParameters().Length > 0
                ? entry.Invoke(null, new object[] { args })
                : entry?.Invoke(null, null);
            old?.Load();
            context.Unload();
            return new WeakReference(context);
        }

        private static void Load(AssemblyLoadContext ctx, Assembly assembly,
            IExtRefResolver resolver, string[] searchPaths)
        {
            foreach (var name in assembly.GetReferencedAssemblies())
            {
                Assembly found;
                try
                {
                    found = ctx.LoadFromAssemblyName(name);
                }
                catch (Exception)
                {
                    found = null;
                }
                if (found != null)
                {
                    Load(ctx, found, resolver, searchPaths);
                    continue;
                }
                foreach (var sp in searchPaths ?? Array.Empty<string>())
                {
                    var spFull = Path.Combine(sp, $"{name.Name}.dll");
                    if (!File.Exists(spFull))
                        continue;
                    var locRef = new LocalRef(spFull);
                    var locPath = resolver.Locate(locRef);
                    found = ctx.LoadFromAssemblyPath(locPath);
                    Load(ctx, found, resolver, searchPaths);
                    break;
                }
                if (found != null)
                    continue;
                var pkgRef = new NuGetRef(name.Name, name.Version?.ToString());
                var pkgPath = resolver.Locate(pkgRef);
                var pkgDll = ctx.LoadFromAssemblyPath(pkgPath);
                Load(ctx, pkgDll, resolver, searchPaths);
            }
        }
    }
}