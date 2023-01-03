using System.CommandLine;
using static TurboDot.Tools.Defaults;

namespace TurboDot.Core
{
    public static class CleanCommandParser
    {
        public static Command GetCommand()
        {
            const string desc = "Clean build outputs of a .NET project";
            var cmd = new Command("clean", desc);
            cmd.AddArgument(SlnOrProjectArgument);
            cmd.SetHandler(CleanCommand.Run);
            return cmd;
        }
    }
}