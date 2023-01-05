using System.IO;
using System.Reflection;

namespace TurboCompile.API.External
{
    public record LocalRef(string FilePath)
        : IExternalRef
    {
        public string FullName => NameObj.FullName;

        public AssemblyName NameObj
            => new(Path.GetFileNameWithoutExtension(FilePath) ?? "_");
    }
}