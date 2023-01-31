using TurboRepo.API;
using TurboRun.Core;

namespace TurboRun.API
{
    public interface IRunner
    {
        bool Execute(byte[] assembly, string[] args,
            Streams streams = null, IExtRefResolver resolver = null,
            string[] searchPaths = null);
    }
}