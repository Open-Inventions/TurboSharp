using TurboCompile.API.External;

namespace TurboCompile.API
{
    public record CompileArgs(
        string[] Paths,
        AssemblyMeta Meta = null,
        IExtRefResolver Resolver = null,
        IExternalRef[] Additional = null,
        bool Debug = true
    );
}