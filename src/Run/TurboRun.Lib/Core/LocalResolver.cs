using System;
using System.IO;
using TurboRepo.API;
using TurboRepo.API.External;

namespace TurboRun.Core
{
    public sealed class LocalResolver : IExtRefResolver
    {
        public string Locate(IExternalRef external)
        {
            if (external is LocalRef lr)
            {
                var path = lr.FilePath;
                if (File.Exists(path))
                    return path;
            }

            throw new InvalidOperationException(external?.ToString());
        }
    }
}