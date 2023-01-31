using System;
using System.Collections.Generic;
using System.Linq;
using Ionide.ProjInfo.Sln.Construction;
using TurboBase.IO;
using TurboMeta.API.Proj;
using TurboMeta.API.Sol;

namespace TurboMeta.Common.Sol
{
    internal sealed class Solution : ISolution
    {
        public Solution(string solFilePath, Func<string, IProject> load)
        {
            FilePath = IoTools.FixSlashFull(solFilePath);
            var real = SolutionFile.Parse(FilePath);

            ProjectsInOrder = real.ProjectsInOrder
                .Where(s => s.ProjectType is
                    SolutionProjectType.KnownToBeMSBuildFormat or
                    SolutionProjectType.WebProject)
                .Select(p => load(p.AbsolutePath));
        }

        public string FilePath { get; }
        public IEnumerable<IProject> ProjectsInOrder { get; }

        public override string ToString() => FilePath;
    }
}