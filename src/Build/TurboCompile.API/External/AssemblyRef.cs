using System.Reflection;

namespace TurboCompile.API.External
{
    public record AssemblyRef(Assembly Assembly)
        : IExternalRef
    {
        public static implicit operator AssemblyRef(Assembly a) => new(a);

        public string FullName => Assembly.FullName;

        public AssemblyName NameObj => Assembly.GetName();
    }
}