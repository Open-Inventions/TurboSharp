using TurboCompile.API.External;

namespace TurboRun
{
    public interface IRunner
    {
        bool Execute(byte[] assembly, string[] args,
            Streams streams = null, IExtRefResolver resolver = null);
    }
}