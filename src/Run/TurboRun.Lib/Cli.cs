using System;
using System.IO;

namespace TurboRun
{
    public static class Cli
    {
        public static void Main(string[] rawArgs)
        {
            var pair = Interactive.Split(rawArgs);
            if (pair == null)
            {
                Console.Error.WriteLine("ERROR: No filename to execute given!");
                return;
            }

            var file = Path.GetFullPath(pair.Value.arg);
            var args = pair.Value.args;

            IRunner runner = new LocalRunner();
            var assembly = File.ReadAllBytes(file);
            var result = runner.Execute(assembly, args);
            Environment.ExitCode = result ? 0 : -1;
        }
    }
}