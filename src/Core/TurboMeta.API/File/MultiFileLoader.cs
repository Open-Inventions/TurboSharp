using System.Collections.Generic;
using TurboMeta.API.Sol;

namespace TurboMeta.API.File
{
    public sealed class MultiFileLoader : IFileLoader<ISolution>
    {
        public IList<IFileLoader<ISolution>> Loaders { get; }

        public MultiFileLoader()
        {
            Loaders = new List<IFileLoader<ISolution>>();
        }

        public ISolution Load(string path, IFileLoader<ISolution> parent = null)
        {
            foreach (var loader in Loaders)
                if (loader.Load(path, parent ?? this) is { } loaded)
                    return loaded;

            return null;
        }
    }
}