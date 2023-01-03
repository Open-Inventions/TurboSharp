﻿using System.CommandLine;
using TurboDot.Core;

namespace TurboDot.Tools
{
    public static class Parser
    {
        public static readonly RootCommand RootCommand =
            new("Execute a .NET SDK command");

        private static readonly Command[] SubCommands =
        {
            BuildCommandParser.GetCommand(),
            CleanCommandParser.GetCommand(),
            RestoreCommandParser.GetCommand()
        };

        public static RootCommand Build(RootCommand rootCommand)
        {
            foreach (var subCommand in SubCommands)
            {
                rootCommand.AddCommand(subCommand);
            }
            return rootCommand;
        }
    }
}