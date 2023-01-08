using System;
using System.IO;
using ByteDev.DotNet.Project;
using Ionide.ProjInfo.Sln.Construction;
using TurboCompile.Common;

namespace TurboDot.Impl
{
    public record ProjectHandle(
        ProjectLang Lang,
        string OriginalPath,
        DotNetProject Proj,
        ProjectInSolution Meta,
        SolutionFile Sol
    )
    {
        public string AbsolutePath
            => Meta == null
                ? OriginalPath
                : IoTools.FixSlash(Meta.AbsolutePath);

        public string ProjectName
            => Meta == null
                ? Path.GetFileNameWithoutExtension(OriginalPath)
                : Meta.ProjectName;

        public string GetFullPath(ProjectReference projRef)
            => throw new InvalidOperationException(); // TODO
    }
}