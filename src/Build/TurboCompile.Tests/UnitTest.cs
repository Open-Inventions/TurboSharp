using System;
using System.IO;
using TurboCompile.API;
using TurboCompile.API.External;
using TurboCompile.Common;
using TurboCompile.CSharp;
using TurboCompile.VBasic;
using TurboDot.Impl;
using TurboRun;
using Xunit;

namespace TurboCompile.Tests
{
    public class UnitTest
    {
        [Theory]
        [InlineData("mincon.vb", 16)]
        [InlineData("mincon.cs", 16)]
        [InlineData("askname.vb", 88)]
        [InlineData("askname.cs", 85)]
        [InlineData("weather.vb", 47)]
        [InlineData("weather.cs", 47)]
        [InlineData("xmly/app.cs", 47774489)]
        [InlineData("xmly/app.vb", 47774489)]
        public void ShouldCompile(string rawFile, int len)
        {
            ICompiler compiler;
            var ext = Path.GetExtension(rawFile);
            var fileName = Path.Combine("Resources", ext[1..].ToUpper(), rawFile);
            switch (ext)
            {
                case CSharpCompiler.Extension:
                    compiler = new CSharpCompiler();
                    break;
                case VBasicCompiler.Extension:
                    compiler = new VBasicCompiler();
                    break;
                default:
                    Assert.Fail(ext);
                    return;
            }

            var paths = new[] { IoTools.FixSlash(fileName) };
            IExtRefResolver resolver = new NuGetResolver();
            var prm = new CompileArgs(paths, Resolver: resolver);
            var (assembly, rtJson) = compiler.Compile(prm);
            Assert.NotNull(rtJson);

            var outDir = Directory.CreateDirectory("Outputs").Name;
            var rawOut = rawFile.Replace('/', '_');
            var outPath = Path.Combine(outDir, $"{rawOut}.dll");
            File.WriteAllBytes(outPath, assembly);

            IRunner runner = new LocalRunner();
            var args = new[] { "Test" };
            var nl = Environment.NewLine;
            var @out = new StringWriter();
            var err = new StringWriter();
            var fake = new Streams(new StringReader($"Test{nl}{nl}"), @out, err);
            var runRes = runner.Execute(assembly, args, fake, resolver);
            Assert.True(runRes);
            Assert.Equal(0, err.ToString().Length);
            var outStr = @out.ToString();
            Assert.True(outStr.Length >= len, $"{outStr.Length} != {len}");
        }
    }
}