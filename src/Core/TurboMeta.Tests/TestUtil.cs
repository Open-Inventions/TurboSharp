using System.IO;
using System.IO.Compression;
using TurboBase.IO;

namespace TurboMeta.Tests
{
    internal static class TestUtil
    {
        public static string ExtractZip(string rawZipFile, string rawZipPath)
        {
            var zipFile = Path.Combine("Resources", IoTools.FixSlash(rawZipFile));
            var zipPath = IoTools.FixSlash(rawZipPath);

            var ownerName = Path.GetFileNameWithoutExtension(zipFile);
            var insideName = Path.GetFileName(zipPath).Replace('.', '-');
            var subName = $"{ownerName[..1]}_{insideName}";

            var outDir = Directory.CreateDirectory("Outputs").Name;
            var outPath = Path.Combine(outDir, subName);
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);

            ZipFile.ExtractToDirectory(zipFile, outPath);

            return Path.Combine(outPath, zipPath);
        }
    }
}