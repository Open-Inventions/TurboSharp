using System.Threading.Tasks;

namespace TurboDot
{
    internal static class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await DotCli.Main(args);
        }
    }
}