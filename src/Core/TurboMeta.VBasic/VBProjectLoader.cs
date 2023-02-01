// ReSharper disable InconsistentNaming

using TurboMeta.Common.Proj;

namespace TurboMeta.VBasic
{
    public sealed class VBProjectLoader : ProjectLoader
    {
        public const string PrjExt = ".vbproj";

        public override string Extension => PrjExt;

        protected override string CodeExtension => VBScriptLoader.CodeExt;
    }
}