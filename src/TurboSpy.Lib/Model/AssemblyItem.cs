using System.Collections.Generic;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class AssemblyItem : SpyItem
    {
        public OneFile One { get; }

        public AssemblyItem(OneFile one) : base(null)
        {
            One = one;
        }

        public override bool CanExpand => true;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield return new ReferencesItem(this, One.ReferencedModules);

            foreach (var (k, v) in One.TopLevelTypeDefs)
            {
                yield return new NameSpaceItem(this, k, v);
            }
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