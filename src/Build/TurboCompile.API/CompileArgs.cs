namespace TurboCompile.API
{
    public record CompileArgs(
        string[] Paths, bool Debug = true
    );
}