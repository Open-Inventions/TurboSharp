using System;
using System.IO;
using TurboCompile.API;
using TurboCompile.CSharp;
using TurboCompile.VBasic;
using TurboRun.Lib;
using Xunit;

namespace TurboCompile.Tests
{
    public class UnitTest
    {
        [Theory]
        [InlineData("mincon.vb", 17)]
        [InlineData("mincon.cs", 17)]
        [InlineData("askname.vb", 90)]
        [InlineData("askname.cs", 90)]
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

            var paths = new[] { fileName };
            var (assembly, rtJson) = compiler.Compile(new CompileArgs(paths));
            Assert.NotNull(rtJson);

            var outDir = Directory.CreateDirectory("Outputs").Name;
            var outPath = Path.Combine(outDir, $"{rawFile}.dll");
            File.WriteAllBytes(outPath, assembly);

            IRunner runner = new LocalRunner();
            var args = new[] { "Test" };
            var nl = Environment.NewLine;
            var @out = new StringWriter();
            var err = new StringWriter();
            var fake = new Streams(new StringReader($"Test{nl}{nl}"), @out, err);
            var runRes = runner.Execute(assembly, args, fake);
            Assert.True(runRes);
            Assert.Equal(0, err.ToString().Length);
            Assert.Equal(len, @out.ToString().Length);
        }
    }
}