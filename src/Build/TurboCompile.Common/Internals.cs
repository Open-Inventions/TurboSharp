using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TurboCompile.Common
{
    public static class Internals
    {
        public static string[] Parse(string prefix, string text, string file)
        {
            var lines = text.Split('\n')
                .TakeWhile(l => !string.IsNullOrWhiteSpace(l))
                .Where(l => l.StartsWith(prefix))
                .Select(l => Parse(l[prefix.Length..], file))
                .ToArray();
            return lines;
        }

        private static string Parse(string line, string file)
        {
            var term = line.Trim().Trim('"');
            var absPath = IoTools.GetAbsPath(term, file);
            return absPath;
        }

        private static readonly Encoding Enc = Encoding.UTF8;

        public static IEnumerable<(string, string)> ReadCode(
            string path, Func<(string, string), string[]> func)
        {
            var full = Path.GetFullPath(path);
            var code = File.ReadAllText(path, Enc);

            var loads = func.Invoke((path, code));
            foreach (var load in loads)
            foreach (var loaded in ReadCode(load, func))
                yield return loaded;

            yield return (full, code);
        }
    }
}