using System.Reflection;

namespace TurboCompile.API.External
{
    public record NuGetRef(string Name, string Version)
        : IExternalRef
    {
        public string FullName => NameObj.FullName;

        public AssemblyName NameObj => new($"nu_{Name}, Version={Version}");
    }
}