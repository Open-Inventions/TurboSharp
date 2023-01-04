namespace TurboCompile.API
{
    public record AssemblyMeta(
        string Name,
        string Version = "1.0.0.0",
        string Company = null,
        string Product = null,
        string Title = null
    );
}