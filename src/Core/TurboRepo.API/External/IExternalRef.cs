using System.Reflection;

namespace TurboRepo.API.External
{
    public interface IExternalRef
    {
        string FullName { get; }

        AssemblyName NameObj { get; }
    }
}