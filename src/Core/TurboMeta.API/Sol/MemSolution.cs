using System.Collections.Generic;
using TurboMeta.API.Proj;

namespace TurboMeta.API.Sol
{
    public sealed class MemSolution : ISolution
    {
        public MemSolution(string path, params IProject[] projects)
        {
            FilePath = path;
            ProjectsInOrder = new List<IProject>(projects);
        }

        public string FilePath { get; }
        public IEnumerable<IProject> ProjectsInOrder { get; }

        public override string ToString() => FilePath;
    }
}