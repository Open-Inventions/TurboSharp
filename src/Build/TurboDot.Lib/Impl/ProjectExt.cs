using System;
using System.Collections.Generic;
using System.IO;
using ByteDev.DotNet.Project;
using Ionide.ProjInfo.Sln.Construction;

namespace TurboDot.Impl
{
    public static class ProjectExt
    {
        public static string GetFullPath(this ProjectInSolution proj, ProjectReference pr)
        {
            var subPath = pr.FilePath;
            var dir = Path.GetDirectoryName(proj.AbsolutePath) ?? string.Empty;
            var full = Path.Combine(dir, subPath);
            full = Path.GetFullPath(full);
            return full;
        }

        public static IEnumerable<ProjectHandle> LoadProjects(this SolutionFile sol)
        {
            foreach (var solProj in sol.ProjectsInOrder)
            {
                if (solProj.ProjectType is not
                    (SolutionProjectType.KnownToBeMSBuildFormat or
                    SolutionProjectType.WebProject))
                    continue;

                var path = solProj.AbsolutePath;
                var ext = Path.GetExtension(path).TrimStart('.');
                Enum.TryParse<ProjectLang>(ext, ignoreCase: true, out var kind);
                if (kind == default)
                    continue;

                var proj = DotNetProject.Load(path);
                yield return new ProjectHandle(kind, proj, solProj, sol);
            }
        }
    }
}