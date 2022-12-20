using System;
using System.IO;
using System.Runtime.CompilerServices;
using TurboSharp.Roslyn.Tools;

namespace TurboSharp.Roslyn
{
    public static class Runner
    {
        public static bool Execute(byte[] assembly, string[] args)
        {
            var weakRef = LoadAndExecute(assembly, args);
            for (var i = 0; i < 8 && weakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return !weakRef.IsAlive;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] assembly, string[] args)
        {
            using var memory = new MemoryStream(assembly);
            var context = new UnloadableContext();
            var loaded = context.LoadFromStream(memory);
            var entry = loaded.EntryPoint;
            _ = entry != null && entry.GetParameters().Length > 0
                ? entry.Invoke(null, new object[] { args })
                : entry?.Invoke(null, null);
            context.Unload();
            return new WeakReference(context);
        }
    }
}