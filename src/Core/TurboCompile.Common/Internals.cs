using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TurboCompile.Common
{
    public static class Internals
    {
        private static readonly Encoding Enc = Encoding.UTF8;

        public static IEnumerable<(string, string)> ReadCode(string path)
        {
            var full = Path.GetFullPath(path);
            var code = File.ReadAllText(full, Enc);
            var res = (full, code);
            yield return res;
        }
    }
}