using TurboCompile.Common;
using MR = Microsoft.CodeAnalysis.MetadataReference;

namespace TurboCompile.Roslyn
{
    public static class MetaTool
    {
        public static AssemblyCache<MR> CreateCache()
        {
            var loader = new MetaRefLoader();
            return new AssemblyCache<MR>(loader);
        }
    }
}