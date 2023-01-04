namespace TurboCompile.API
{
    public record CompileArgs(
        string[] Paths,
        AssemblyMeta Meta = null,
        bool Debug = true
    );
}