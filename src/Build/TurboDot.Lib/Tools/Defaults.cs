using System.Collections.Generic;
using System.CommandLine;

namespace TurboDot.Tools
{
    internal static class Defaults
    {
        private const string SolutionOrProjectArgName
            = "PROJECT | SOLUTION";

        private const string SolutionOrProjectArgument
            = "The project or solution file to operate on. " +
              "If a file is not specified, the command " +
              "will search the current directory for one.";

        public static readonly Argument<IEnumerable<string>> SlnOrProjectArgument
            = new(SolutionOrProjectArgName)
            {
                Description = SolutionOrProjectArgument,
                Arity = ArgumentArity.ZeroOrMore
            };

        private const string NoRestore
            = "Do not restore the project before building.";

        public static Option<bool> NoRestoreOption
            = new("--no-restore", NoRestore);
    }
}