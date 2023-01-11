using System.IO;

namespace TurboCompile.Common
{
    public static class IoTools
    {
        private static readonly char Sep = Path.DirectorySeparatorChar;

        public static string FixSlash(string path)
        {
            return Sep switch
            {
                '\\' => path.Replace('/', Sep),
                '/' => path.Replace('\\', Sep),
                _ => path
            };
        }

        public static string GetPathPart(string name)
        {
            return $"{Sep}{name}{Sep}";
        }

        public static string GetAbsPath(string term, string file)
        {
            var relPath = FixSlash(term);
            var fileDir = Path.GetDirectoryName(file) ?? string.Empty;
            var absPath = Path.Combine(fileDir, relPath);
            absPath = Path.GetFullPath(absPath);
            return absPath;
        }
    }
}