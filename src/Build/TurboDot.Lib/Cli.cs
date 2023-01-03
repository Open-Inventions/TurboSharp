using System.CommandLine;
using System.Threading.Tasks;
using TurboDot.Tools;

namespace TurboDot
{
    public static class Cli
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = Parser.Build(Parser.RootCommand);
            return await rootCommand.InvokeAsync(args);
        }
    }
}