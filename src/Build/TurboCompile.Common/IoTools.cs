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
    }
}