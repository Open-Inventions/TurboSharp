// ReSharper disable InconsistentNaming

using TurboMeta.Common.File;

namespace TurboMeta.CSharp
{
    internal sealed class CSScript : Script
    {
        public CSScript(string filePath) : base(filePath)
        {
            ParseCode(FilePath, "//");
        }
    }
}