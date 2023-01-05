using System.Reflection;

namespace TurboCompile.API.External
{
    public interface IExternalRef
    {
        string FullName { get; }

        AssemblyName NameObj { get; }
    }
}