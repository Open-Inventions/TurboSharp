// ReSharper disable InconsistentNaming

using TurboMeta.Common.Proj;

namespace TurboMeta.VBasic
{
    public sealed class VBProjectLoader : ProjectLoader
    {
        public override string Extension => ".vbproj";

        protected override string CodeExtension => VBScriptLoader.CodeExt;
    }
}