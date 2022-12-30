using System;
using System.Collections.Generic;
using ICSharpCode.Decompiler.TypeSystem;

namespace TurboSpy.Model
{
    public class TypeDefItem : SpyItem
    {
        private readonly ITypeDefinition _def;

        public TypeDefItem(SpyItem parent, ITypeDefinition def) : base(parent)
        {
            _def = def;
        }

        public override bool CanExpand => false;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield break;
        }

        protected override string Text => $"[{Kind}] {_def.Name}";

        public FullTypeName TypeName => _def.FullTypeName;

        private string Kind
        {
            get
            {
                var kind = _def.Kind;
                switch (kind)
                {
                    case TypeKind.Class:
                        return "C";
                    case TypeKind.Enum:
                        return "E";
                    case TypeKind.Interface:
                        return "I";
                    case TypeKind.Struct:
                        return "S";
                    default:
                        throw new InvalidOperationException($"{kind} ?!");
                }
            }
        }
    }
}