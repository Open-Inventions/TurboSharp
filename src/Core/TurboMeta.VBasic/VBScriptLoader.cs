// ReSharper disable InconsistentNaming

using TurboMeta.API.Proj;
using TurboMeta.Common.File;

namespace TurboMeta.VBasic
{
    public sealed class VBScriptLoader : ScriptLoader
    {
        public const string CodeExt = ".vb";

        public override string Extension => CodeExt;

        protected override IProject LoadScript(string path)
        {
            var script = new VBScript(path);
            return script;
        }
    }
}