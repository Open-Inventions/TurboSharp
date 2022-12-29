namespace TurboSpy.Model
{
    public class AssemblyItem : SpyItem
    {
        public OneFile One { get; }

        public AssemblyItem(OneFile one)
        {
            One = one;
        }

        protected override string Text
        {
            get
            {
                var name = One.FullAssemblyName;
                return $" {name.Name} ({name.Version})";
            }
        }
    }
}