// ReSharper disable InconsistentNaming

using TurboMeta.Common.Proj;

namespace TurboMeta.CSharp
{
    public sealed class CSProjectLoader : ProjectLoader
    {
        public const string PrjExt = ".csproj";

        public override string Extension => PrjExt;

        protected override string CodeExtension => CSScriptLoader.CodeExt;
    }
}