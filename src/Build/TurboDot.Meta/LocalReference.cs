namespace TurboDot.Meta
{
    public class LocalReference
    {
        public string Name { get; internal set; }

        public string Path { get; internal set; }

        public override string ToString() => $"{Name} [{Path}]";
    }
}