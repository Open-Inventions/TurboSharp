using System.Collections.Generic;

namespace TurboSpy.Model
{
    public abstract class SpyItem
    {
        public SpyItem Parent { get; }

        protected SpyItem(SpyItem parent)
        {
            Parent = parent;
        }

        public abstract bool CanExpand { get; }
        public abstract IEnumerable<SpyItem> GetChildren();
        protected abstract string Text { get; }

        public override string ToString() => $" {Text}";
    }
}