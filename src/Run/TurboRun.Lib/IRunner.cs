namespace TurboRun.Lib
{
    public interface IRunner
    {
        bool Execute(byte[] assembly, string[] args, Streams streams = null);
    }
}