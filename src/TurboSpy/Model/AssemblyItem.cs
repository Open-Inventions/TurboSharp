using System.Collections.Generic;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class AssemblyItem : SpyItem
    {
        public OneFile One { get; }

        public AssemblyItem(OneFile one)
        {
            One = one;
        }

        public override bool CanExpand => true;

        public override IEnumerable<SpyItem> GetChildren()
        {
            var file = One.Decompiler.TypeSystem.MainModule.PEFile;
            yield return new ReferencesItem(One.ReferencedModules, file);
        }

        protected override string Text
        {
            get
            {
                var name = One.FullAssemblyName;
                return $"{name.Name} ({name.Version})";
            }
        }
    }
}