using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terminal.Gui;
using Terminal.Gui.Trees;
using TurboBase.IO;
using TurboBase.UI;
using TurboSpy.Core;
using TurboSpy.Model;

namespace TurboSpy.View
{
    internal sealed class MainTopLevel : Toplevel
    {
        private readonly Env _boot;
        private readonly IDictionary<string, OneFile> _files;
        private readonly Decompiler _parent;
        private readonly TreeView<SpyItem> _treeView;
        private readonly TextView _textView;

        public MainTopLevel(Env boot)
        {
            _boot = boot;
            _files = new SortedDictionary<string, OneFile>();
            _parent = new Decompiler();
            ColorScheme = Visuals.GetBaseColor();
            MenuBar = CreateMenuBar();
            var blue = Visuals.CreateTextColor();
            var (tree, text, winL, winR) = CreateTree(blue, blue);
            _treeView = tree;
            _textView = text;
            var status = CreateStatus();
            Add(MenuBar);
            Add(winL);
            Add(winR);
            Add(status);

        }

        private StatusBar CreateStatus()
        {
            var statusBar = new StatusBar(Array.Empty<StatusItem>());
            return statusBar;
        }

        private (TreeView<SpyItem>, TextView, Window, Window) CreateTree(ColorScheme wColor, ColorScheme tColor)
        {
            var winL = new Window("Assemblies")
            {
                X = 0,
                Y = 1,
                Width = Dim.Percent(35),
                Height = Dim.Fill(),
                ColorScheme = wColor
            };
            var treeView = new TreeView<SpyItem>
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ColorScheme = tColor
            };
            treeView.TreeBuilder = new DelegateTreeBuilder<SpyItem>(GetChild, CanExpand);
            treeView.SelectionChanged += OnTreeSelect;
            winL.Add(treeView);

            var winR = new Window("Decompiled")
            {
                X = Pos.Right(winL),
                Y = 1,
                Width = Dim.Percent(65),
                Height = Dim.Fill(),
                ColorScheme = wColor
            };
            var textView = new TextView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1,
                ColorScheme = tColor
            };
            textView.ClearKeybinding(Key.AltMask | Key.F);
            winR.Add(textView);

            return (treeView, textView, winL, winR);
        }

        private bool CanExpand(SpyItem arg) => arg.CanExpand;
        private IEnumerable<SpyItem> GetChild(SpyItem arg) => arg.GetChildren();

        private void OnTreeSelect(object sender, SelectionChangedEventArgs<SpyItem> e)
        {
            var selected = e.NewValue;
            if (selected is AssemblyItem ai)
            {
                var moduleMeta = ai.One.GetModuleTxt();
                _textView.Text = moduleMeta;
                return;
            }
            if (selected is ReferencesItem ri)
            {
                var refsMeta = ri.GetListTxt();
                _textView.Text = refsMeta;
                return;
            }
            if (selected is ReferenceItem rfi)
            {
                var refMeta = rfi.GetListTxt();
                _textView.Text = refMeta;
                return;
            }
            if (selected is NameSpaceItem ni)
            {
                var nspMeta = ni.GetListTxt();
                _textView.Text = nspMeta;
                return;
            }
            if (selected is TypeDefItem ti)
            {
                var code = ((AssemblyItem)ti.Parent).One.Decompile(ti);
                _textView.Text = code;
                return;
            }
            if (selected is PropertyItem pi)
            {
                var code = ((AssemblyItem)pi.Parent).One.Decompile(pi.Property);
                _textView.Text = code;
                return;
            }
            if (selected is MethodItem mi)
            {
                var code = ((AssemblyItem)mi.Parent).One.Decompile(mi.Method);
                _textView.Text = code;
                return;
            }
            if (selected is FieldItem fi)
            {
                var code = ((AssemblyItem)fi.Parent).One.Decompile(fi.Field);
                _textView.Text = code;
                return;
            }
            if (selected is EventItem ei)
            {
                var code = ((AssemblyItem)ei.Parent).One.Decompile(ei.Event);
                _textView.Text = code;
                return;
            }
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
                new MenuBarItem("_View", new[]
                {
                    new MenuItem("_Collapse all nodes", null, DoCollapse)
                }),
                new MenuBarItem("_Help", new[]
                {
                    new MenuItem("_About...", null, DoAbout)
                })
            });
        }

        private void DoCollapse()
        {
            _treeView.CollapseAll();
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
            if (!IoTools.IsValidFile(currentFile))
            {
                ShowError(currentFile);
                return;
            }
            var one = LoadFile(currentFile);
            _files[currentFile] = one;
            RefreshTree(one);
        }

        private static void ShowError(string file)
        {
            MessageBox.ErrorQuery("Load error", file, "_OK");
        }

        private void RefreshTree(OneFile one)
        {
            var wrap = new AssemblyItem(one);
            _treeView.AddObject(wrap);
            if (_treeView.Objects.Count() <= 1)
                _treeView.GoToFirst();
            _treeView.SetNeedsDisplay();
            _treeView.SetFocus();
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
            message.AppendLine(@"Copyright (C) 2023 by");
            message.AppendLine(@"");
            message.AppendLine(@"Open Inventions, Inc.");
            return message.ToString();
        }
    }
}