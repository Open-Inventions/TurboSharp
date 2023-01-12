using System.IO;
using ByteDev.DotNet.Project;
using Ionide.ProjInfo.Sln.Construction;
using TurboCompile.Common;
using TurboDot.Meta;

namespace TurboDot.Impl
{
    public record ProjectHandle(
        ProjectLang Lang,
        string OriginalPath,
        NetProject Proj,
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
            => IoTools.GetAbsPath(projRef.FilePath, AbsolutePath);

        public string GetFullPath(LocalReference locRef)
            => IoTools.GetAbsPath(locRef.Path, AbsolutePath);
    }
}