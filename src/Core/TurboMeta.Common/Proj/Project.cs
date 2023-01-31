using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ByteDev.DotNet.Project;
using TurboBase.IO;
using TurboMeta.API.Proj;
using TurboMeta.API.Ref;
using PackageReference = TurboMeta.API.Ref.PackageReference;
using ProjectReference = TurboMeta.API.Ref.ProjectReference;

namespace TurboMeta.Common.Proj
{
    internal sealed class Project : IProject
    {
        public Project(string prjFilePath, string codeExt)
        {
            FilePath = IoTools.FixSlashFull(prjFilePath);
            var doc = XDocument.Load(FilePath);
            var real = new DotNetProject(doc);

            Name = Path.GetFileNameWithoutExtension(FilePath);

            var root = doc.Root;
            Sdk = root?.Attribute("Sdk")?.Value;

            var props = root?.Element("PropertyGroup");
            var outType = props?.Element("OutputType")?.Value;
            if (Enum.TryParse<OutputMode>(outType, true, out var ot))
                OutputMode = ot;

            LocalReferences = root?.Descendants("Reference").Select(ParseRef);
            PackageReferences = real.PackageReferences.Select(p =>
                new PackageReference(p.Name, p.Version));
            ProjectReferences = real.ProjectReferences.Select(p =>
                new ProjectReference(p.FilePath));
            ContentReferences = Array.Empty<ContentReference>();
            IncludedItems = ListFiles(ItemDir, codeExt);
        }

        public string FilePath { get; }
        public string Name { get; }
        public string Sdk { get; }
        public OutputMode OutputMode { get; }
        public IEnumerable<PackageReference> PackageReferences { get; }
        public IEnumerable<ProjectReference> ProjectReferences { get; }
        public IEnumerable<LocalReference> LocalReferences { get; }
        public IEnumerable<ContentReference> ContentReferences { get; }
        public IEnumerable<string> IncludedItems { get; }

        private string ItemDir => Path.GetFullPath(Path.GetDirectoryName(FilePath)!);

        private static IEnumerable<string> ListFiles(string dir, string filter)
        {
            const SearchOption o = SearchOption.AllDirectories;
            var files = Directory.GetFiles(dir, filter, o);
            return files;
        }

        private static LocalReference ParseRef(XElement element)
        {
            var name = element.Attribute("Include")?.Value;
            var path = element.Element("HintPath")?.Value;
            return new LocalReference(Name: name, FilePath: path);
        }

        public override string ToString() => FilePath;
    }
}