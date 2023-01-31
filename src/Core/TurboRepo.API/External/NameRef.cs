using System.Reflection;

namespace TurboRepo.API.External
{
    public record NameRef(string Name)
        : IExternalRef
    {
        public string FullName => NameObj.FullName;

        public AssemblyName NameObj => new(Name);
    }
}