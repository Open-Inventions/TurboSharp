using System;
using Terminal.Gui;
using TurboBase.UI;
using TurboSharp.View;

namespace TurboSharp
{
    public static class SharpCli
    {
        public static int Main(string[] args)
        {
            var boot = new Env
            {
                Root = Environment.CurrentDirectory,
                Args = args
            };

            Application.Init();
            Application.Run(CreateTop(boot));
            Application.Shutdown();

            return 0;
        }

        public static Toplevel CreateTop(Env boot) => new MainTopLevel(boot);
    }
}