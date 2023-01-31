using System;
using TurboCompile.API;
using TurboMeta.API.File;
using TurboMeta.API.Proj;
using TurboMeta.Common.Sol;
using TurboMeta.CSharp;
using TurboMeta.VBasic;

namespace TurboDot.Tools
{
    public static class DotUtil
    {
        public static MultiFileLoader CreateLoader()
        {
            var loader = new MultiFileLoader();
            var loaders = loader.Loaders;
            loaders.Add(new SolutionLoader());
            loaders.Add(new VBProjectLoader());
            loaders.Add(new VBScriptLoader());
            loaders.Add(new CSProjectLoader());
            loaders.Add(new CSScriptLoader());
            return loader;
        }

        public static OutputType ToKind(this OutputMode type)
        {
            switch (type)
            {
                case OutputMode.Lib:
                    return OutputType.Library;
                case OutputMode.Exe:
                    return OutputType.Console;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}