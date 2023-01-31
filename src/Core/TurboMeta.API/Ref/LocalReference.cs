namespace TurboMeta.API.Ref
{
    public record LocalReference(
        string Name,
        string FilePath
    ) : IFileReference
    {
        public override string ToString() => $"{Name} [{FilePath}]";
    }
}