using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace TurboRun
{
    public sealed class LocalRunner : IRunner
    {
        public bool Execute(byte[] assembly, string[] args, Streams streams = null)
        {
            var weakRef = LoadAndExecute(assembly, args, streams);
            for (var i = 0; i < 8 && weakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return !weakRef.IsAlive;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] assembly, string[] args, Streams con)
        {
            using var memory = new MemoryStream(assembly);
            var context = new UnloadableContext();
            var loaded = context.LoadFromStream(memory);
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
    }
}