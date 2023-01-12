using System;
using Microsoft.CodeAnalysis;
using TurboCompile.API;

namespace TurboCompile.Roslyn
{
    public static class OptionTool
    {
        public static OutputKind ToKind(this OutputType type)
        {
            switch (type)
            {
                case OutputType.None:
                case OutputType.Console:
                    return OutputKind.ConsoleApplication;
                case OutputType.Library:
                    return OutputKind.DynamicallyLinkedLibrary;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}