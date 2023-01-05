using System.Reflection;

namespace TurboCompile.API.External
{
    public record NameRef(string Name)
        : IExternalRef
    {
        public string FullName => NameObj.FullName;

        public AssemblyName NameObj => new(Name);
    }
}