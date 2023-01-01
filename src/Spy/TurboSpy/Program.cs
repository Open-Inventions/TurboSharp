using System;
using Terminal.Gui;
using TurboSharp.Common;

namespace TurboSpy
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var boot = new Env
            {
                Root = Environment.CurrentDirectory,
                Args = args
            };

            Application.Init();
            Application.Run(new MainTopLevel(boot));
            Application.Shutdown();
        }
    }
}