using System.Linq;

namespace TurboRun
{
    public static class Interactive
    {
        public static (string arg, string[] args)? Split(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                return null;
            }
            var first = args.First();
            var second = args.Skip(1).ToArray();
            return (first, second);
        }
    }
}