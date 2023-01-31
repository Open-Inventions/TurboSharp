using System;
using System.IO;
using System.Text;
using TurboMeta.API.Ref;

namespace TurboMeta.Common.Util
{
    public static class ScriptTools
    {
        private static readonly Encoding Utf = Encoding.UTF8;

        public static void Load(string filePath, string prefix, Action<string> handler)
        {
            using var reader = new StreamReader(filePath, Utf);
            string line;
            while (!string.IsNullOrWhiteSpace(line = reader.ReadLine()))
            {
                if (!line.StartsWith(prefix))
                    continue;
                handler(line[prefix.Length..]);
            }
        }

        public static void ParseRef(string line, out PackageReference pr,
            out LocalReference lr, out ContentReference fr)
        {
            pr = null;
            lr = null;
            fr = null;
            var tmp = "#r ";
            if (line.StartsWith(tmp))
            {
                var refLine = line[tmp.Length..].Trim().Trim('"');
                tmp = "nuget: ";
                if (refLine.StartsWith(tmp))
                {
                    var nugLine = refLine[tmp.Length..];
                    var nugParts = nugLine.Split(',');
                    var nugName = nugParts[0].Trim();
                    var nugVer = nugParts[1].Trim();
                    pr = new PackageReference(nugName, nugVer);
                }
                else
                {
                    var lrPath = refLine.Trim();
                    var lrName = Path.GetFileNameWithoutExtension(lrPath);
                    lr = new LocalReference(lrName, lrPath);
                }
            }
            tmp = "#load ";
            if (line.StartsWith(tmp))
            {
                var ldLine = line[tmp.Length..].Trim().Trim('"');
                fr = new ContentReference(ldLine);
            }
        }
    }
}