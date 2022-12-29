namespace TurboSpy.Model
{
    public abstract class SpyItem
    {
        protected abstract string Text { get; }

        public override string ToString() => Text;
    }
}