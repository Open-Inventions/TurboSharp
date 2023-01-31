using System;
using Microsoft.CodeAnalysis;
using TurboCompile.API;
using TurboCompile.Common;
using MR = Microsoft.CodeAnalysis.MetadataReference;

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

        public static AssemblyCache<MR> CreateCache()
        {
            var loader = new MetaRefLoader();
            return new AssemblyCache<MR>(loader);
        }
    }
}