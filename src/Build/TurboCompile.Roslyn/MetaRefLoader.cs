using TurboCompile.API;
using MR = Microsoft.CodeAnalysis.MetadataReference;

namespace TurboCompile.Roslyn
{
    public sealed class MetaRefLoader : IRefLoader<MR>
    {
        public MR LoadFrom(byte[] bytes)
        {
            return MR.CreateFromImage(bytes);
        }

        public MR LoadFrom(string path)
        {
            return MR.CreateFromFile(path);
        }
    }
}