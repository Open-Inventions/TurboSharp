// ReSharper disable InconsistentNaming

using TurboMeta.API.Proj;
using TurboMeta.Common.File;

namespace TurboMeta.CSharp
{
    public sealed class CSScriptLoader : ScriptLoader
    {
        public const string CodeExt = ".cs";

        public override string Extension => CodeExt;

        protected override IProject LoadScript(string path)
        {
            var script = new CSScript(path);
            return script;
        }
    }
}