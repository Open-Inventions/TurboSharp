using System;
using System.IO;
using System.Linq;
using TurboRun.Lib;

namespace TurboRun
{
    internal static class Program
    {
        private static void Main(string[] rawArgs)
        {
            if (rawArgs == null || rawArgs.Length == 0)
            {
                Console.Error.WriteLine("ERROR: No filename to execute given!");
                return;
            }

            var file = Path.GetFullPath(rawArgs.First());
            var args = rawArgs.Skip(1).ToArray();

            IRunner runner = new LocalRunner();
            var assembly = File.ReadAllBytes(file);
            runner.Execute(assembly, args);
        }
    }
}