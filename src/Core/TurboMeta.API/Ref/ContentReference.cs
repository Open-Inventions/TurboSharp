namespace TurboMeta.API.Ref
{
    public record ContentReference(
        string FilePath
    ) : IFileReference
    {
        public override string ToString() => FilePath;
    }
}