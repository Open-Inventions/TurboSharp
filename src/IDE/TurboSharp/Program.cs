using System;
using Terminal.Gui;
using TurboRun;
using TurboSharp.Common;

using IdeTopLevel = TurboSharp.MainTopLevel;
using SpyTopLevel = TurboSpy.MainTopLevel;
using RunCli = TurboRun.Cli;

namespace TurboSharp
{
    internal static class Program
    {
        private static void Main(string[] rawArgs)
        {
            var split = Interactive.Split(rawArgs);
            var cmd = split?.arg;
            Enum.TryParse<MainCommand>(cmd, ignoreCase: true, out var mode);
            var args = split == null || mode == default
                ? rawArgs
                : split.Value.args;

            if (mode == MainCommand.Run)
            {
                RunCli.Main(args);
                return;
            }

            var boot = new Env
            {
                Root = Environment.CurrentDirectory,
                Args = args
            };

            Application.Init();
            Toplevel top = mode == MainCommand.Spy
                ? new SpyTopLevel(boot)
                : new IdeTopLevel(boot);
            Application.Run(top);
            Application.Shutdown();
        }
    }
}