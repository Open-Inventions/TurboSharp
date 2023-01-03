using System.Threading.Tasks;

namespace TurboDot
{
    internal static class Program
    {
        private static async Task<int> Main(string[] rawArgs)
        {
            return await Cli.Main(rawArgs);
        }
    }
}