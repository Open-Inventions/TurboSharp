namespace TurboCompile.API
{
    public record CompileResult(
        byte[] RawAssembly,
        string RuntimeJson
    );
}