using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using TurboDot.Tools;
using TurboMeta.API.Proj;

namespace TurboDot.Core
{
    public static class CleanCommand
    {
        public static void Run(InvocationContext obj)
        {
            Run(obj.ParseResult);
        }

        private static void Run(ParseResult result)
        {
            var files = DotCli.GetSlnOrProject(result);
            if (files == null)
            {
                DotCli.ShowSlnOrProjectError();
                return;
            }

            var loader = DotUtil.CreateLoader();
            foreach (var handle in files.SelectMany(f =>
                         DotCli.ReadSlnOrProject(loader, f).ProjectsInOrder))
            {
                var abs = handle.FilePath;
                LogSink.Write(@$" Cleaning project ""{abs}""...");
                var dir = handle.GetFolder();
                var bin = Directory.GetDirectories(dir, "bin")
                    .FirstOrDefault();
                if (bin != null)
                    CleanFolder(bin);
                var obj = Directory.GetDirectories(dir, "obj")
                    .FirstOrDefault();
                if (obj != null)
                    CleanFolder(obj);
                LogSink.Write(@$" Done with project ""{abs}"".");
            }
        }

        private static void CleanFolder(string dir)
        {
            const SearchOption o = SearchOption.AllDirectories;
            var files = Directory.GetFiles(dir, "*.*", o);
            foreach (var file in files)
            {
                var txt = @$"  Deleting file ""{file}""";
                LogSink.Write(txt);
                File.Delete(file);
            }
        }
    }
}