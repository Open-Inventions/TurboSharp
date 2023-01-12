using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ByteDev.DotNet.Project;

namespace TurboDot.Meta
{
    public class NetProject : DotNetProject
    {
        public NetProject(XDocument doc) : base(doc)
        {
            var root = doc.Root;
            SdkName = root?.Attribute("Sdk")?.Value;

            var props = root?.Element("PropertyGroup");
            var outType = props?.Element("OutputType")?.Value;
            if (Enum.TryParse<ProjOutput>(outType, true, out var ot))
                OutputType = ot;

            LocalReferences = root?.Descendants("Reference")
                .Select(r =>
                {
                    var name = r.Attribute("Include")?.Value;
                    var path = r.Element("HintPath")?.Value;
                    return new LocalReference { Name = name, Path = path };
                });
        }

        public string SdkName { get; }
        public ProjOutput OutputType { get; }
        public IEnumerable<LocalReference> LocalReferences { get; }

        public new static NetProject Load(string projFilePath)
            => new(XDocument.Load(projFilePath));
    }
}