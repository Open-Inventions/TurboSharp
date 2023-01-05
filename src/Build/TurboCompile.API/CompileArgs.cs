using TurboCompile.API.External;

namespace TurboCompile.API
{
    public record CompileArgs(
        string[] Paths,
        AssemblyMeta Meta = null,
        IExtRefResolver Resolver = null,
        bool Debug = true
    );
}