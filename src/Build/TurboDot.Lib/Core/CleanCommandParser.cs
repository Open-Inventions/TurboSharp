using System.CommandLine;

namespace TurboDot.Core
{
    public static class CleanCommandParser
    {
        public static Command GetCommand()
        {
            var cmd = new Command("clean", "Clean build outputs of a .NET project");
            return cmd;
        }
    }
}