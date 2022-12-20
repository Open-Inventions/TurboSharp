using System.Reflection;
using System.Runtime.Loader;

namespace TurboSharp.Roslyn.Tools
{
    public sealed class UnloadableContext : AssemblyLoadContext
    {
        public UnloadableContext()
            : base(true)
        {
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            return null;
        }
    }
}