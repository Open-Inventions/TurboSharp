using System;
using System.IO;
using TurboRepo.API;
using TurboRun.API;
using TurboRun.Core;
using TurboRun.Tools;

namespace TurboRun
{
    public static class RunCli
    {
        public static int Main(string[] rawArgs, Streams streams = null)
        {
            var pair = Interactive.Split(rawArgs);
            if (pair == null)
            {
                Console.Error.WriteLine("ERROR: No filename to execute given!");
                return -1;
            }

            var file = Path.GetFullPath(pair.Value.arg);
            var args = pair.Value.args;

            var root = Path.GetDirectoryName(file);
            var roots = new[] { root };

            IRunner runner = new LocalRunner();
            IExtRefResolver find = new LocalResolver();
            var assembly = File.ReadAllBytes(file);
            var result = runner.Execute(assembly, args, streams, find, roots);

            return result ? 0 : -1;
        }
    }
}