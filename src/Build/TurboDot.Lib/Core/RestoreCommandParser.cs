using System.CommandLine;
using static TurboDot.Tools.Defaults;

namespace TurboDot.Core
{
    public static class RestoreCommandParser
    {
        public static Command GetCommand()
        {
            const string desc = "Restore dependencies specified in a .NET project";
            var cmd = new Command("restore", desc);
            cmd.AddArgument(SlnOrProjectArgument);
            cmd.SetHandler(RestoreCommand.Run);
            return cmd;
        }
    }
}