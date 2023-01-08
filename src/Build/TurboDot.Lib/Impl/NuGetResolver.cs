using System;
using System.IO;
using System.Linq;
using TurboCompile.API.External;
using static TurboCompile.Common.Externals;
using static TurboCompile.Common.IoTools;

namespace TurboDot.Impl
{
    public sealed class NuGetResolver : IExtRefResolver
    {
        private readonly bool _allowDownload;
        private readonly NuGet _nuGet;

        public NuGetResolver(NuGet nuGet = null, bool allowDownload = true)
        {
            _allowDownload = allowDownload;
            _nuGet = nuGet ?? new NuGet(nameof(NuGet).ToLower());
        }

        public string Locate(IExternalRef external)
        {
            switch (external)
            {
                case AssemblyRef ar:
                    var aLoc = ar.Assembly.Location;
                    return aLoc;
                case LocalRef lr:
                    var lLoc = Path.GetFullPath(FixSlash(lr.FilePath));
                    return lLoc;
                case NameRef nr:
                    var nLoc = LoadByName(nr.NameObj).Location;
                    return nLoc;
                case NuGetRef ur:
                    var uTask = _allowDownload
                        ? _nuGet.Download(ur.Name, ur.Version)
                        : _nuGet.FindMatch(ur.Name, ur.Version);
                    var uPath = uTask.GetAwaiter().GetResult();
                    var possible = new[]
                    {
                        "net6.0", "netstandard2.1", "netstandard2.0"
                    };
                    var candidate = possible
                        .Select(p => Path.Combine(uPath, "lib", p))
                        .Where(Directory.Exists)
                        .First();
                    var uLoc = Directory.GetFiles(candidate, "*.dll")
                        .FirstOrDefault();
                    return uLoc;
                default:
                    throw new ArgumentOutOfRangeException(nameof(external));
            }
        }
    }
}