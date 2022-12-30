using System.Collections.Generic;
using ICSharpCode.Decompiler.TypeSystem;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class FieldItem : SpyItem
    {
        public FieldItem(SpyItem parent, IField field) : base(parent)
        {
            Field = field;
        }

        public IField Field { get; }

        public override bool CanExpand => false;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield break;
        }

        protected override string Text
            => $"{Field.Name} : {Field.Type.ToSimple()}";
    }
}