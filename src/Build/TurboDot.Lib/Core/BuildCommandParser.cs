using System.CommandLine;
using static TurboDot.Tools.Defaults;

namespace TurboDot.Core
{
    public static class BuildCommandParser
    {
        public static Command GetCommand()
        {
            const string desc = "Build a .NET project";
            var cmd = new Command("build", desc);
            cmd.AddArgument(SlnOrProjectArgument);
            cmd.AddOption(NoRestoreOption);
            cmd.SetHandler(BuildCommand.Run);
            return cmd;
        }
    }
}