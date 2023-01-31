using System.IO;
using TurboBase.IO;
using TurboMeta.API.Ref;

namespace TurboMeta.API.Proj
{
    public static class Extensions
    {
        public static string GetFolder(this IProject proj)
            => Path.GetDirectoryName(proj.FilePath);

        public static string GetFullPath(this IProject proj, IFileReference projRef)
            => IoTools.GetAbsPath(projRef.FilePath, proj.FilePath);
    }
}