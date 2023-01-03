using System.CommandLine;

namespace TurboDot.Core
{
    public static class RestoreCommandParser
    {
        public static Command GetCommand()
        {
            var cmd = new Command("restore", "Restore dependencies specified in a .NET project");
            return cmd;
        }
    }
}