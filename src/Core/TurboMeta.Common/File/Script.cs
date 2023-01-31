using System;
using System.Collections.Generic;
using System.IO;
using TurboBase.IO;
using TurboMeta.API.Proj;
using TurboMeta.API.Ref;
using TurboMeta.Common.Util;

namespace TurboMeta.Common.File
{
    public abstract class Script : IProject
    {
        protected Script(string filePath)
        {
            FilePath = IoTools.FixSlashFull(filePath);

            Name = Path.GetFileNameWithoutExtension(FilePath);
            OutputMode = OutputMode.Exe;
            Sdk = Defaults.StandardSdk;

            ProjectReferences = Array.Empty<ProjectReference>();
            PackageReferences = new List<PackageReference>();
            LocalReferences = new List<LocalReference>();
            ContentReferences = new List<ContentReference>();
            IncludedItems = new List<string>();
        }

        public string FilePath { get; }
        public string Name { get; }
        public string Sdk { get; }
        public OutputMode OutputMode { get; }
        public IEnumerable<PackageReference> PackageReferences { get; protected set; }
        public IEnumerable<ProjectReference> ProjectReferences { get; protected set; }
        public IEnumerable<LocalReference> LocalReferences { get; protected set; }
        public IEnumerable<ContentReference> ContentReferences { get; protected set; }
        public IEnumerable<string> IncludedItems { get; protected set; }

        protected void ParseCode(string filePath, string prefix)
        {
            ScriptTools.Load(filePath, prefix, l =>
            {
                ScriptTools.ParseRef(l, out var pr, out var lr, out var fr);
                if (pr != null && PackageReferences is ICollection<PackageReference> cpr)
                    cpr.Add(pr);
                if (lr != null && LocalReferences is ICollection<LocalReference> lpr)
                    lpr.Add(lr);
                if (fr != null && ContentReferences is ICollection<ContentReference> fpr)
                    fpr.Add(fr);
            });

            if (IncludedItems is ICollection<string> fpr)
            {
                fpr.Add(FilePath);
                foreach (var cr in ContentReferences)
                {
                    var rcr = IoTools.GetAbsPath(cr.FilePath, filePath);
                    fpr.Add(rcr);
                }
            }
        }

        public override string ToString() => FilePath;
    }
}