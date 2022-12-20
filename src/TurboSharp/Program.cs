using System;
using Terminal.Gui;

namespace TurboSharp
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Env.Root = Environment.CurrentDirectory;
            Env.Args = args;

            Application.Init();
            Application.Run<MainTopLevel>();
            Application.Shutdown();
        }
    }
}