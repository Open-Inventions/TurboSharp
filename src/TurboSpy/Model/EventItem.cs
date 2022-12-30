using System.Collections.Generic;
using ICSharpCode.Decompiler.TypeSystem;
using TurboSpy.Core;

namespace TurboSpy.Model
{
    public class EventItem : SpyItem
    {
        public EventItem(SpyItem parent, IEvent @event) : base(parent)
        {
            Event = @event;
        }

        public IEvent Event { get; }

        public override bool CanExpand => false;

        public override IEnumerable<SpyItem> GetChildren()
        {
            yield break;
        }

        protected override string Text 
            => $"{Event.Name} : {Event.ReturnType.ToSimple()}";
    }
}