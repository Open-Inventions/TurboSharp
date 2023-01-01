using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace TurboSpy.Model
{
    public class ReferenceItem : SpyItem
    {
        private readonly AssemblyName _module;

        public ReferenceItem(SpyItem parent, AssemblyName module) : base(parent)
        {
            _module = module;
        }

        public override bool CanExpand => false;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield break;
        }

        protected override string Text => _module.Name;

        public string GetListTxt()
        {
            var bld = new StringBuilder();
            bld.AppendLine();
            var full = $"// {_module.FullName}";
            bld.AppendLine(full);
            bld.AppendLine();
            return bld.ToString();
        }
    }
}