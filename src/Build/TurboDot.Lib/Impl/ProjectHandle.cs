using ByteDev.DotNet.Project;
using Ionide.ProjInfo.Sln.Construction;

namespace TurboDot.Impl
{
    public record ProjectHandle(
        ProjectLang Lang,
        DotNetProject Proj,
        ProjectInSolution Meta,
        SolutionFile Sol
    );
}