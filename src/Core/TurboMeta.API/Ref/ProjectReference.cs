namespace TurboMeta.API.Ref
{
    public record ProjectReference(
        string FilePath
    ) : IFileReference
    {
        public override string ToString() => FilePath;
    }
}