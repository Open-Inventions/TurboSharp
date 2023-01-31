using System;
using TurboDot;
using TurboRun;
using TurboRun.Tools;
using TurboSharp.Core;
using TurboSpy;

namespace TurboSharp
{
    internal static class Program
    {
        private static int Main(string[] rawArgs)
        {
            var split = Interactive.Split(rawArgs);
            var cmd = split?.arg;
            Enum.TryParse<MainCommand>(cmd, ignoreCase: true, out var mode);

            var args = split == null || mode == default
                ? rawArgs
                : split.Value.args;

            if (mode == MainCommand.Run)
            {
                return RunCli.Main(args);
            }

            if (mode == MainCommand.Dot)
            {
                return DotCli.Main(args).GetAwaiter().GetResult();
            }

            if (mode == MainCommand.Spy)
            {
                return SpyCli.Main(args);
            }

            return SharpCli.Main(args);
        }
    }
}