// ReSharper disable InconsistentNaming

using TurboMeta.Common.File;

namespace TurboMeta.VBasic
{
    internal sealed class VBScript : Script
    {
        public VBScript(string filePath) : base(filePath)
        {
            ParseCode(FilePath, "'");
        }
    }
}