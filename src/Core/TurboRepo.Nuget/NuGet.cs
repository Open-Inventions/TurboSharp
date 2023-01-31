using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TurboBase.IO;

namespace TurboRepo.Nuget
{
    public sealed class NuGet
    {
        private static readonly HttpClient Client = new();

        private readonly string _api;
        private readonly string _host;
        private readonly string _root;

        public NuGet(string root = "nuget")
        {
            _root = root;
            _host = "https://www.nuget.org";
            _api = "https://api.nuget.org/v3/index.json";
        }

        public Task<string> FindMatch(string name, string ver)
        {
            var pkgPath = GetPath(name, ver);
            var pkgDir = Path.GetDirectoryName(pkgPath)!;
            return Task.FromResult(pkgDir);
        }

        public async Task<string> Download(string name, string ver)
        {
            var pkgPath = GetPath(name, ver);
            if (!File.Exists(pkgPath))
            {
                await StoreZip(pkgPath, name, ver);
            }
            var pkgHashPath = $"{pkgPath}.sha512";
            if (!File.Exists(pkgHashPath))
            {
                var checksum = GetCheckSum(pkgPath);
                await File.WriteAllTextAsync(pkgHashPath, checksum);
            }
            var pkgDir = Path.GetDirectoryName(pkgPath)!;
            await Extract(pkgPath, pkgDir);
            return pkgDir;
        }

        private static async Task Extract(string pkgPath, string pkgDir)
        {
            using var pkgZip = ZipFile.OpenRead(pkgPath);
            foreach (var entry in pkgZip.Entries)
            {
                var entryName = entry.Name;
                if (entryName == "[Content_Types].xml")
                    continue;
                var entryFull = entry.FullName;
                if (entryFull.StartsWith("_rels/"))
                    continue;
                if (entryFull.StartsWith("package/"))
                    continue;

                var entryPath = IoTools.FixSlash(entryFull);
                var entryDest = Path.Combine(pkgDir, entryPath);
                if (File.Exists(entryDest))
                    continue;

                var entryDir = Path.GetDirectoryName(entryDest)!;
                if (!Directory.Exists(entryDir))
                    Directory.CreateDirectory(entryDir!);

                await using var zipOut = File.Create(entryDest);
                await using var zipStream = entry.Open();
                await zipStream.CopyToAsync(zipOut);
                await zipOut.FlushAsync();
                File.SetLastWriteTime(entryDest, entry.LastWriteTime.DateTime);
            }
        }

        private async Task StoreZip(string pkgPath, string name, string ver)
        {
            var baseUrl = new Uri($"{_host}/api/v2/package/{name}/{ver}");
            using var response = await Client.GetAsync(baseUrl);
            response.EnsureSuccessStatusCode();
            await using var pkgStream = await response.Content.ReadAsStreamAsync();
            await using var pkgOut = File.Create(pkgPath);
            await pkgStream.CopyToAsync(pkgOut);
            await pkgOut.FlushAsync();
        }

        private string GetPath(string name, string ver)
        {
            var nugetDir = Directory.CreateDirectory(_root).FullName;
            var label = name.ToLower();
            var pkgDir = Path.Combine(nugetDir, label, ver);
            if (!Directory.Exists(pkgDir))
                Directory.CreateDirectory(pkgDir);
            var pkgFile = $"{label}.{ver}.nupkg";
            var pkgPath = Path.Combine(pkgDir, pkgFile);
            return pkgPath;
        }

        private static string GetCheckSum(string filePath)
        {
            using var hash = SHA512.Create();
            using var stream = File.OpenRead(filePath);
            return Convert.ToBase64String(hash.ComputeHash(stream));
        }
    }
}