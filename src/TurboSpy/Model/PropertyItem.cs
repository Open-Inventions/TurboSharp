using System.Collections.Generic;
using ICSharpCode.Decompiler.TypeSystem;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class PropertyItem : SpyItem
    {
        public PropertyItem(SpyItem parent, IProperty property) : base(parent)
        {
            Property = property;
        }

        public IProperty Property { get; }

        public override bool CanExpand => false;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield break;
        }

        protected override string Text 
            => $"{Property.Name} : {Property.ReturnType.ToSimple()}";
    }
}