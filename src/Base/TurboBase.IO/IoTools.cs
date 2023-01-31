using System.IO;

namespace TurboBase.IO
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

        public static bool IsValidFile(string fileName)
        {
            return !string.IsNullOrWhiteSpace(fileName)
                   && File.Exists(fileName)
                   && File.GetCreationTime(fileName) != default;
        }

        public static string ResolveOrCreate(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                return null;
            var dir = Path.GetFullPath(folder);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }

        public static string FixSlashFull(string path)
        {
            var correct = FixSlash(path);
            return Path.GetFullPath(correct);
        }
    }
}