using System.CommandLine;

namespace TurboDot.Core
{
    public static class BuildCommandParser
    {
        public static Command GetCommand()
        {
            var cmd = new Command("build", "Build a .NET project");
            return cmd;
        }
    }
}