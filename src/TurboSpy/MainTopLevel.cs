using System.Collections.Generic;
using System.IO;
using System.Text;
using Terminal.Gui;
using TurboSharp.Common;
using TurboSpy.Core;
using TurboSpy.Model;

namespace TurboSpy
{
    internal sealed class MainTopLevel : Toplevel
    {
        private readonly Env _boot;
        private readonly IDictionary<string, OneFile> _files;
        private readonly Decompiler _parent;

        public MainTopLevel(Env boot)
        {
            _boot = boot;
            _files = new SortedDictionary<string, OneFile>();
            _parent = new Decompiler();
            ColorScheme = Visuals.GetBaseColor();
            MenuBar = CreateMenuBar();
            Add(MenuBar);
        }

        private MenuBar CreateMenuBar()
        {
            return new MenuBar(new[]
            {
                new MenuBarItem("_File", new[]
                {
                    new MenuItem("_Open...", null, DoOpen,
                        null, null, Key.CtrlMask | Key.O),
                    null,
                    new MenuItem("E_xit", null, DoExit,
                        null, null, Key.AltMask | Key.X)
                }),
                new MenuBarItem("_Help", new[]
                {
                    new MenuItem("_About...", null, DoAbout)
                })
            });
        }

        private void DoOpen()
        {
            var allowed = new List<string> { ".dll", ".exe" };
            var dialog = new OpenDialog("Open", "Choose a single file or more.", allowed)
            {
                AllowsMultipleSelection = false
            };
            Application.Run(dialog);

            if (dialog.Canceled || dialog.FilePaths.Count <= 0)
                return;

            var currentFile = dialog.FilePaths[0];
            AddFile(currentFile);
        }

        private void AddFile(string rawFile)
        {
            var currentFile = Path.GetFullPath(rawFile);
            if (!Inputs.IsValidFile(currentFile))
                return;
            _files[currentFile] = LoadFile(currentFile);
        }

        private OneFile LoadFile(string fileName)
        {
            var decompiler = _parent.GetDecompiler(fileName);
            var one = new OneFile(decompiler);
            return one;
        }

        private void DoExit()
        {
            RequestStop();
        }

        private static void DoAbout()
        {
            MessageBox.Query("About", GetAboutMessage(), "_OK");
        }

        private static string GetAboutMessage()
        {
            var message = new StringBuilder();
            message.AppendLine(@"");
            message.AppendLine(@"Turbo Spy");
            message.AppendLine(@"");
            message.AppendLine(@"Version 0.1");
            message.AppendLine(@"");
            message.AppendLine(@"Copyright (C) 2022 by");
            message.AppendLine(@"");
            message.AppendLine(@"Open Inventions, Inc.");
            return message.ToString();
        }
    }
}