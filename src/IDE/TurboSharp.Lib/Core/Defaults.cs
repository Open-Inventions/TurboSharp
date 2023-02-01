using System.Collections.Generic;
using TurboMeta.Common.Sol;
using TurboMeta.CSharp;
using TurboMeta.VBasic;

namespace TurboSharp.Core
{
    internal static class Defaults
    {
        internal static List<string> GetAllowedExtensions()
        {
            return new List<string>
            {
                CSScriptLoader.CodeExt,
                VBScriptLoader.CodeExt,
                CSProjectLoader.PrjExt,
                VBProjectLoader.PrjExt,
                SolutionLoader.SolExt
            };
        }
    }
}