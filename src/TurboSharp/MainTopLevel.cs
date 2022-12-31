using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terminal.Gui;
using TurboSharp.Common;
using TurboSharp.Roslyn;
using TurboSpy;

namespace TurboSharp
{
    internal sealed class MainTopLevel : Toplevel
    {
        private readonly Env _boot;

        private const string EmptyFile = "Untitled";
        private readonly TextView _textView;
        private string _currentFileName;

        public MainTopLevel(Env boot)
        {
            _boot = boot;
            ColorScheme = Visuals.GetBaseColor();
            MenuBar = CreateMenuBar();
            var blue = Visuals.CreateTextColor();
            var (textWin, textView, statusBar) = CreateTextView(EmptyFile, blue, blue);
            _textView = textView;
            Add(MenuBar);
            Add(textWin);
            Add(statusBar);
        }

        private static (Window, TextView, StatusBar) CreateTextView(string title,
            ColorScheme wColor, ColorScheme tColor)
        {
            var win = new Window(title)
            {
                X = 0, Y = 1,
                Width = Dim.Fill(), Height = Dim.Fill(),
                ColorScheme = wColor
            };
            var textView = new TextView
            {
                X = 0, Y = 0,
                Width = Dim.Fill(), Height = Dim.Fill(),
                BottomOffset = 1, RightOffset = 1,
                ColorScheme = tColor
            };
            textView.ClearKeybinding(Key.AltMask | Key.F);
            var pos = new StatusItem(Key.Null, "", null);
            textView.UnwrappedCursorPosition += e => pos.Title = $"[{e.Y + 1}:{e.X + 1}]";
            var statusBar = new StatusBar(new[] { pos });
            win.Add(textView);
            return (win, textView, statusBar);
        }

        private MenuBar CreateMenuBar()
        {
            return new MenuBar(new[]
            {
                new MenuBarItem("_File", new[]
                {
                    new MenuItem("_New...", null, DoNew,
                        null, null, Key.CtrlMask | Key.N),
                    new MenuItem("_Open...", null, DoOpen,
                        null, null, Key.CtrlMask | Key.O),
                    new MenuItem("_Save all", null, DoSaveAll,
                        CanSaveAll, null, Key.CtrlMask | Key.S),
                    null,
                    new MenuItem("E_xit", null, DoExit,
                        null, null, Key.AltMask | Key.X)
                }),
                new MenuBarItem("_Edit", new[]
                {
                    new MenuItem("Cu_t", null, DoCut,
                        CanCut, null, Key.CtrlMask | Key.X),
                    new MenuItem("_Copy", null, DoCopy,
                        CanCopy, null, Key.CtrlMask | Key.C),
                    new MenuItem("_Paste", null, DoPaste,
                        CanPaste, null, Key.CtrlMask | Key.V)
                }),
                new MenuBarItem("_Search", new[]
                {
                    new MenuItem("_Find...", null, DoFind,
                        CanFind, null, Key.CtrlMask | Key.T)
                }),
                new MenuBarItem("_Run", new[]
                {
                    new MenuItem("_Run", null, DoRun,
                        CanRun, null, Key.CtrlMask | Key.F9)
                }),
                new MenuBarItem("_Compile", new[]
                {
                    new MenuItem("_Compile", null, DoCompile,
                        CanCompile, null, Key.CtrlMask | Key.F6)
                }),
                new MenuBarItem("Too_ls", new[]
                {
                    new MenuItem("Turbo_Spy", null, DoSpy,
                        null, null, Key.CtrlMask | Key.ShiftMask | Key.F7)
                }),
                new MenuBarItem("_Help", new[]
                {
                    new MenuItem("_About...", null, DoAbout)
                })
            });
        }

        private void DoSpy()
        {
            var spy = new TurboSpy.MainTopLevel(_boot);
            Application.Run(spy);
        }

        private bool CanSaveAll()
        {
            return false; // TODO
        }

        private void DoSaveAll()
        {
            throw new System.NotImplementedException();
        }

        private bool CanBeClosed()
        {
            return true; // TODO
        }

        private void DoOpen()
        {
            if (!CanBeClosed())
                return;

            var allowed = new List<string> { ".cs" };
            var dialog = new OpenDialog("Open", "Choose a single file or more.", allowed)
            {
                AllowsMultipleSelection = false
            };
            Application.Run(dialog);

            if (dialog.Canceled || dialog.FilePaths.Count <= 0)
                return;

            _currentFileName = dialog.FilePaths[0];
            LoadFile();
        }

        private void LoadFile()
        {
            if (string.IsNullOrWhiteSpace(_currentFileName))
                return;

            _textView.LoadFile(_currentFileName);
            var window = (Window)_textView.SuperView.SuperView;
            window.Title = _currentFileName;
        }

        private void DoNew()
        {
            throw new System.NotImplementedException();
        }

        private bool CanPaste()
        {
            return false; // TODO
        }

        private bool CanCopy()
        {
            return false; // TODO
        }

        private bool CanCut()
        {
            return false; // TODO
        }

        private void DoPaste()
        {
            _textView?.Paste();
        }

        private void DoCopy()
        {
            _textView?.Copy();
        }

        private void DoCut()
        {
            _textView?.Cut();
        }

        private bool CanFind()
        {
            return false; // TODO
        }

        private void DoFind()
        {
            throw new System.NotImplementedException("TODO");
        }

        private bool CanCompile()
        {
            return Inputs.IsValidFile(_currentFileName);
        }

        private void DoCompile()
        {
            if (!Inputs.IsValidFile(_currentFileName))
                return;
            
            CompileOrRun(nameof(Compiler), false);
        }

        private bool CanRun()
        {
            return Inputs.IsValidFile(_currentFileName);
        }

        private void DoRun()
        {
            if (!Inputs.IsValidFile(_currentFileName))
                return;
            
            CompileOrRun(nameof(Runner), true);
        }

        private void CompileOrRun(string prefix, bool run)
        {
            var args = Array.Empty<string>();
            Exception error = null;
            byte[] assembly = null;
            string json = null;
            try
            {
                (assembly, json) = Compiler.Compile(_currentFileName);
            }
            catch (Exception e)
            {
                error = e;
            }

            var oldOut = Console.Out;
            var oldErr = Console.Error;

            using var newOut = new StringWriter();
            using var newErr = new StringWriter();
            Console.SetOut(newOut);
            Console.SetOut(newErr);

            var okay = assembly != null && error == null
                                        && (!run || Runner.Execute(assembly, args));

            Console.SetOut(oldOut);
            Console.SetError(oldErr);

            var nl = Environment.NewLine;
            var output = (error?.Message + nl + newOut + nl + newErr).Trim();
            if (!run && assembly != null)
            {
                var dir = Path.GetDirectoryName(_currentFileName);
                var label = Path.GetFileNameWithoutExtension(_currentFileName);
                var exeName = $"{label}.exe";
                var exeMeta = $"{label}.runtimeconfig.json";
                var metaPath = Path.Combine(dir, exeMeta);
                File.WriteAllText(metaPath, json, Encoding.UTF8);
                var exePath = Path.Combine(dir, exeName);
                File.WriteAllBytes(exePath, assembly);
                output = (exePath + nl + output).Trim();
            }
            var add = okay ? "Success" : "Failure";
            MessageBox.Query($"{prefix} {add}", output, "_OK");
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
            message.AppendLine(@"Turbo Sharp");
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