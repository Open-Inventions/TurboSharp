using System.Collections.Generic;
using TurboMeta.API.Ref;

namespace TurboMeta.API.Proj
{
    public interface IProject
    {
        string FilePath { get; }

        string Name { get; }

        string Sdk { get; }

        OutputMode OutputMode { get; }

        IEnumerable<PackageReference> PackageReferences { get; }

        IEnumerable<ProjectReference> ProjectReferences { get; }

        IEnumerable<LocalReference> LocalReferences { get; }

        IEnumerable<ContentReference> ContentReferences { get; }

        IEnumerable<string> IncludedItems { get; }
    }
}