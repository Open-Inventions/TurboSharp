// ReSharper disable InconsistentNaming

using TurboMeta.Common.Proj;

namespace TurboMeta.CSharp
{
    public sealed class CSProjectLoader : ProjectLoader
    {
        public override string Extension => ".csproj";

        protected override string CodeExtension => CSScriptLoader.CodeExt;
    }
}