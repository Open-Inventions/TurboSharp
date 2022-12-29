using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.Decompiler.Metadata;

namespace TurboSpy.Model
{
    public class ReferencesItem : SpyItem
    {
        private readonly AssemblyName[] _modules;
        private readonly PEFile _file;

        public ReferencesItem(IEnumerable<AssemblyName> modules, PEFile file)
        {
            _modules = modules.ToArray();
            _file = file;
        }

        public override bool CanExpand => _modules.Any();

        public override IEnumerable<SpyItem> GetChildren()
        {
            foreach (var module in _modules)
            {
                yield return new ReferenceItem(module);
            }
        }

        protected override string Text => "References";

        public string GetListTxt()
        {
            var bld = new StringBuilder();
            var meta = _file.Metadata.DetectTargetFrameworkId();
            bld.AppendLine();
            bld.AppendLine($"// Detected Target-Framework-Id: {meta}");
            bld.AppendLine();
            bld.AppendLine("// Referenced assemblies:");
            foreach (var mod in _modules)
            {
                bld.AppendLine($"//  {mod}");
            }
            bld.AppendLine();
            return bld.ToString();
        }
    }
}