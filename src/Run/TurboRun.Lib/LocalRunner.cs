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
            Streams streams = null, IExtRefResolver resolver = null)
        {
            var weakRef = LoadAndExecute(assembly, args, streams, resolver);
            for (var i = 0; i < 8 && weakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return !weakRef.IsAlive;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] assembly, string[] args,
            Streams con, IExtRefResolver resolver)
        {
            using var memory = new MemoryStream(assembly);
            var context = new UnloadableContext();
            var loaded = context.LoadFromStream(memory);
            Load(context, loaded, resolver);
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
            IExtRefResolver resolver)
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
                    Load(ctx, found, resolver);
                    continue;
                }
                var pkgRef = new NuGetRef(name.Name, name.Version?.ToString());
                var pkgPath = resolver.Locate(pkgRef);
                var pkgDll = ctx.LoadFromAssemblyPath(pkgPath);
                Load(ctx, pkgDll, resolver);
            }
        }
    }
}