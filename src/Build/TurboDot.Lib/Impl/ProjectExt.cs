using System;
using System.Collections.Generic;
using System.IO;
using ByteDev.DotNet.Project;
using Ionide.ProjInfo.Sln.Construction;
using TurboCompile.Common;

namespace TurboDot.Impl
{
    public static class ProjectExt
    {
        private static string GetAbsPath(this ProjectInSolution proj)
            => IoTools.FixSlash(proj.AbsolutePath);

        public static string GetFullPath(this ProjectInSolution proj, ProjectReference pr)
        {
            var subPath = IoTools.FixSlash(pr.FilePath);
            var dir = Path.GetDirectoryName(GetAbsPath(proj)) ?? string.Empty;
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

                var item = LoadProject(solProj, sol);
                yield return item;
            }
        }

        private static ProjectHandle LoadProject(ProjectInSolution solProj, SolutionFile sol)
        {
            return LoadProject(GetAbsPath(solProj), solProj, sol);
        }

        public static ProjectHandle LoadProject(string path,
            ProjectInSolution solProj, SolutionFile sol)
        {
            var ext = Path.GetExtension(path).TrimStart('.');
            Enum.TryParse<ProjectLang>(ext, ignoreCase: true, out var kind);
            if (kind == default)
                return null;

            var proj = DotNetProject.Load(path);
            return new ProjectHandle(kind, proj, solProj, sol);
        }

        public static string GetFolder(this ProjectHandle file)
        {
            var path = GetFile(file);
            return Path.GetDirectoryName(path);
        }

        public static string GetFile(this ProjectHandle file)
        {
            var path = file.Meta.GetAbsPath();
            return path;
        }
    }
}