using System.IO;

namespace TurboSharp.Common
{
    public static class Inputs
    {
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
    }
}