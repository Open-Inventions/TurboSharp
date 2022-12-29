using System.Collections.Generic;

namespace TurboSpy.Model
{
    public abstract class SpyItem
    {
        public abstract bool CanExpand { get; }
        public abstract IEnumerable<SpyItem> GetChildren();
        protected abstract string Text { get; }

        public override string ToString() => $" {Text}";
    }
}