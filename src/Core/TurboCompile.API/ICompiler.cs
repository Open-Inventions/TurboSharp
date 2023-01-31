namespace TurboCompile.API
{
    public interface ICompiler
    {
        CompileResult Compile(CompileArgs args);
    }
}