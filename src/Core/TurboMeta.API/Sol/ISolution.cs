using System.Collections.Generic;
using TurboMeta.API.Proj;

namespace TurboMeta.API.Sol
{
    public interface ISolution
    {
        string FilePath { get; }

        IEnumerable<IProject> ProjectsInOrder { get; }
    }
}