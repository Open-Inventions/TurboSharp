using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using TurboCompile.Common;
using Xunit;

namespace TurboDot.Tests
{
    public class UnitTest
    {
        [Theory]
        [InlineData("Resty.zip", @"Resty.sln")]
        [InlineData("Resty.zip", @"RestyC/RestyC.csproj")]
        [InlineData("Resty.zip", @"RestyV/RestyV.vbproj")]
        public async Task ShouldBuild(string zipFile, string rawZipPath)
        {
            var fileName = Path.Combine("Resources", zipFile);
            var zipPath = IoTools.FixSlash(rawZipPath);
            var subName = Path.GetFileNameWithoutExtension(zipPath);

            var outDir = Directory.CreateDirectory("Outputs").Name;
            var outPath = Path.Combine(outDir, subName);
            if (Directory.Exists(outPath))
                Directory.Delete(outPath, true);
            Directory.CreateDirectory(outPath);

            ZipFile.ExtractToDirectory(fileName, outPath);

            var fullPath = Path.Combine(outPath, zipPath);
            var res = await Cli.Main(new[] { "build", fullPath });
            Assert.Equal(0, res);
        }
    }
}