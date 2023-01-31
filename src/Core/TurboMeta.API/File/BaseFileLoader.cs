using System.IO;
using TurboMeta.API.Sol;

namespace TurboMeta.API.File
{
    public abstract class BaseFileLoader : IFileLoader<ISolution>
    {
        public abstract string Extension { get; }

        protected abstract ISolution SafeLoad(
            string path, IFileLoader<ISolution> parent = null
        );

        public ISolution Load(string path, IFileLoader<ISolution> parent = null)
        {
            if (!string.IsNullOrWhiteSpace(path) &&
                Extension.Equals(Path.GetExtension(path)))
            {
                return SafeLoad(path, parent);
            }
            return null;
        }
    }
}