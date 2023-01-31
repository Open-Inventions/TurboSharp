namespace TurboMeta.API.Ref
{
    public record PackageReference(
        string Name,
        string Version
    )
    {
        public override string ToString() => $"{Name} v{Version}";
    }
}