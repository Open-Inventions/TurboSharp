namespace TurboBase.UI
{
    public record Env
    {
        public string[] Args { get; init; }

        public string Root { get; init; }
    }
}