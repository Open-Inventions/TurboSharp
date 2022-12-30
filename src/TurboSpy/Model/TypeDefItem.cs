using System;
using System.Collections.Generic;
using System.Linq;
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

        public override bool CanExpand => true;

        public override IEnumerable<SpyItem> GetChildren()
        {
            foreach (var member in _def.Members)
            {
                if (member is IField f)
                {
                    if (f.Name.EndsWith("__BackingField"))
                        continue;
                    yield return new FieldItem(Parent, f);
                }
                else if (member is IMethod m)
                {
                    if (m.GetType().Name == "FakeMethod" || m.Name.StartsWith("<"))
                        continue;
                    yield return new MethodItem(Parent, m);
                }
                else if (member is IProperty p)
                {
                    yield return new PropertyItem(Parent, p);
                }
                else if (member is IEvent e)
                {
                    yield return new EventItem(Parent, e);
                }
                else
                    throw new InvalidOperationException($"{member} {member.SymbolKind}");
            }
        }

        protected override string Text => $"[{Kind}] {_def.Name}{Suffix}";

        public string Suffix
        {
            get
            {
                var prm = string.Join(",", _def.TypeParameters
                    .Select(t => t.Name));
                return prm.Length == 0 ? string.Empty : $"<{prm}>";
            }
        }

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
                    case TypeKind.Delegate:
                        return "D";
                    default:
                        throw new InvalidOperationException($"{kind} ?!");
                }
            }
        }
    }
}