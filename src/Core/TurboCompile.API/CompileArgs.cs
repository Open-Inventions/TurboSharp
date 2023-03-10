using TurboRepo.API;
using TurboRepo.API.External;

namespace TurboCompile.API
{
    public record CompileArgs(
        string[] Paths,
        OutputType Kind,
        AssemblyMeta Meta = null,
        IExtRefResolver Resolver = null,
        IExternalRef[] Additional = null,
        bool Debug = true
    );
}