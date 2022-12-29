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
    }
}