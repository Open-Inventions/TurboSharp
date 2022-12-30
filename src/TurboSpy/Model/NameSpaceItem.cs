using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Decompiler.TypeSystem;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class NameSpaceItem : SpyItem
    {
        private readonly string _name;
        private readonly ITypeDefinition[] _typeDefs;

        public NameSpaceItem(SpyItem parent, string name, ITypeDefinition[] typeDefs)
            : base(parent)
        {
            _name = name;
            _typeDefs = typeDefs;
        }

        public override bool CanExpand => _typeDefs.Any();

        public override IEnumerable<SpyItem> GetChildren()
        {
            foreach (var def in _typeDefs)
            {
                yield return new TypeDefItem(Parent, def);
            }
        }

        protected override string Text => _name;

        public string GetListTxt()
        {
            var bld = new StringBuilder();
            bld.AppendLine();
            var real = _name.Replace(OneFile.RootNamespace, string.Empty);
            var full = $"// {real}";
            bld.AppendLine(full);
            bld.AppendLine();
            return bld.ToString();
        }
    }
}