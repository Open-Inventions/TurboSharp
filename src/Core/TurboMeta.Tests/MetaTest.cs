using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TurboDot;
using TurboDot.Tools;
using TurboMeta.API.Proj;
using TurboMeta.Common.Util;
using TurboRun;
using TurboRun.Core;
using Xunit;
using static TurboMeta.Common.Sol.SolutionLoader;
using static TurboMeta.Tests.TestUtil;

namespace TurboMeta.Tests
{
    public sealed class MetaTest
    {
        [Theory]
        [InlineData("single/mincon.zip", "mincon.cs", 1, 0, 0, 0, 0, "mincon", 1, 16)]
        [InlineData("single/mincon.zip", "mincon.vb", 1, 0, 0, 0, 0, "mincon", 1, 16)]
        [InlineData("single/askname.zip", "askname.cs", 1, 0, 0, 0, 0, "askname", 1, 86)]
        [InlineData("single/askname.zip", "askname.vb", 1, 0, 0, 0, 0, "askname", 1, 86)]
        [InlineData("single/weather.zip", "weather.cs", 1, 0, 2, 0, 0, "weather", 1, 46)]
        [InlineData("single/weather.zip", "weather.vb", 1, 0, 2, 0, 0, "weather", 1, 46)]
        [InlineData("single/xmly.zip", "app.cs", 1, 1, 0, 0, 1, "app", 2, 172, true)]
        [InlineData("single/xmly.zip", "app.vb", 1, 1, 0, 0, 1, "app", 2, 172, true)]
        [InlineData("multi/Resty.zip", "Resty.sln", 2, 0, 4, 0, 0, "RestyC RestyV", 2, 46)]
        [InlineData("multi/Resty.zip", "RestyC/RestyC.csproj", 1, 0, 2, 0, 0, "RestyC", 1, 46)]
        [InlineData("multi/Resty.zip", "RestyV/RestyV.vbproj", 1, 0, 2, 0, 0, "RestyV", 1, 46)]
        [InlineData("multi/Xmly.zip", "Xmly.sln", 5, 2, 0, 2, 0, "XmlyC XmlyC.Model XmlyV XmlyV.Model Xmly.Embed", 9, 172, true, "Exe Lib")]
        [InlineData("multi/Xmly.zip", "Xmly.Embed/Xmly.Embed.csproj", 1, 0, 0, 0, 0, "Xmly.Embed", 1, null, false, "Lib")]
        [InlineData("multi/Xmly.zip", "XmlyC.Model/XmlyC.Model.csproj", 1, 0, 0, 0, 0, "XmlyC.Model", 3, null, false, "Lib")]
        [InlineData("multi/Xmly.zip", "XmlyC/XmlyC.csproj", 2, 1, 0, 1, 0, "XmlyC XmlyC.Model", 4, 172, true, "Exe Lib")]
        [InlineData("multi/Xmly.zip", "XmlyV.Model/XmlyV.Model.vbproj", 1, 0, 0, 0, 0, "XmlyV.Model", 3, null, false, "Lib")]
        [InlineData("multi/Xmly.zip", "XmlyV/XmlyV.vbproj", 2, 1, 0, 1, 0, "XmlyV XmlyV.Model", 4, 172, true, "Exe Lib")]
        public async Task ShouldLoad(string rawZipFile, string rawZipPath,
            int prjCount, int lrCount, int paCount, int prCount, int coCount,
            string names, int itCount, int? len, bool ignoreWeak = false,
            string modes = "Exe")
        {
            var fullPath = ExtractZip(rawZipFile, rawZipPath);

            var loader = DotUtil.CreateLoader();
            var loaded = loader.Load(fullPath);

            Assert.Equal(loaded.FilePath, loaded.ToString());
            Assert.EndsWith(fullPath
                    .Replace(".csproj", SolExt)
                    .Replace(".cs", SolExt)
                    .Replace(".vbproj", SolExt)
                    .Replace(".vb", SolExt),
                loaded.FilePath);

            var projects = loaded.ProjectsInOrder.ToArray();
            Assert.Equal(prjCount, projects.Length);

            var sdk = projects.Select(p => p.Sdk).Distinct();
            Assert.Equal(Defaults.StandardSdk, sdk.Single());

            var mode = projects.Select(p => p.OutputMode).Distinct();
            Assert.Equal(modes, string.Join(" ", mode));

            var name = projects.Select(p => p.Name);
            Assert.Equal(names, string.Join(" ", name));

            var fpText = projects.SelectMany(p => p.FilePath);
            var toText = projects.SelectMany(p => p.ToString());
            Assert.Equal(fpText, toText);

            var lr = projects
                .SelectMany(p => p.LocalReferences).ToArray();
            var pa = projects
                .SelectMany(p => p.PackageReferences).ToArray();
            var pr = projects
                .SelectMany(p => p.ProjectReferences).ToArray();
            var co = projects
                .SelectMany(p => p.ContentReferences).ToArray();
            var it = projects
                .SelectMany(p => p.IncludedItems).ToArray();
            Assert.Equal((lrCount, paCount, prCount, coCount, itCount),
                (lr.Length, pa.Length, pr.Length, co.Length, it.Length));

            var dotRes = await DotCli.Main(new[] { "clean", fullPath });
            Assert.Equal(0, dotRes);

            dotRes = await DotCli.Main(new[] { "restore", fullPath });
            Assert.Equal(0, dotRes);

            dotRes = await DotCli.Main(new[] { "build", fullPath });
            Assert.Equal(0, dotRes);

            var projPath = projects.FirstOrDefault(p =>
                p.OutputMode == OutputMode.Exe)?.FilePath;
            if (projPath == null)
                return;
            Run(projPath, len ?? 1_000_000, ignoreWeak);
        }

        private static void Run(string path, int len, bool ignoreWeak)
        {
            var dir = Path.GetDirectoryName(path) ?? string.Empty;
            dir = Path.Combine(dir, "bin", "Debug", "net6.0");
            var exeName = Path.GetFileNameWithoutExtension(path);
            var fullPath = Path.Combine(dir, $"{exeName}.dll");

            var nl = Environment.NewLine;
            var @out = new StringWriter();
            var err = new StringWriter();
            var fake = new Streams(new StringReader($"Test{nl}{nl}"), @out, err);

            var args = new[] { fullPath, "Test" };
            var runRes = RunCli.Main(args, fake);

            Assert.Equal(ignoreWeak ? -1 : 0, runRes);
            Assert.Equal(0, err.ToString().Length);
            var outStr = @out.ToString();
            Assert.True(outStr.Length >= len, $"{outStr.Length} != {len}");
        }

        [Fact]
        public void ShouldFailLoad()
        {
            var loader = DotUtil.CreateLoader();
            var result = loader.Load("test.tmp");
            Assert.Null(result);
        }
    }
}