using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class MethodItem : SpyItem
    {
        public MethodItem(SpyItem parent, IMethod method) : base(parent)
        {
            Method = method;
        }

        public IMethod Method { get; }

        public override bool CanExpand => false;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield break;
        }

        protected override string Text
        {
            get
            {
                var name = Method.Name;
                var prm = string.Join(", ", Method.Parameters
                    .Select(p => p.Type.ToSimple()));
                var ret = $" : {Method.ReturnType.ToSimple()}";
                var arg = string.Join(",", Method.TypeArguments
                    .Select(t => t.ToSimple()));
                var args = arg.Length == 0 ? string.Empty : $"<{arg}>";
                if (Method.SymbolKind == SymbolKind.Constructor)
                {
                    name = Method.DeclaringType.Name;
                    ret = null;
                }
                return $"{name}{args}({prm}){ret}";
            }
        }
    }
}