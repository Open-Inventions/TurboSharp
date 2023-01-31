using System.Linq;
using TurboMeta.API.File;
using TurboMeta.API.Proj;
using TurboMeta.API.Sol;

namespace TurboMeta.Common.Sol
{
    public sealed class SolutionLoader : BaseFileLoader
    {
        public const string SolExt = ".sln";

        public override string Extension => SolExt;

        protected override ISolution SafeLoad(string path,
            IFileLoader<ISolution> parent = null)
        {
            var owner = parent ?? this;

            IProject LoadProj(string p) => owner
                .Load(p, owner)
                .ProjectsInOrder.First();

            var sol = new Solution(path, LoadProj);
            return sol;
        }
    }
}