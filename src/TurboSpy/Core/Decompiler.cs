using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX.PdbProvider;

namespace TurboSpy.Core
{
    internal sealed class Decompiler
    {
        public LanguageVersion LanguageVersion { get; set; }
        public bool RemoveDeadCode { get; set; }
        public bool RemoveDeadStores { get; set; }
        public bool NestedDirectories { get; set; }
        public bool ShowIlSequencePointsFlag { get; set; }
        public IList<string> ReferencePaths { get; set; }

        public Decompiler()
        {
            LanguageVersion = LanguageVersion.Latest;
            RemoveDeadCode = true;
            RemoveDeadStores = true;
            NestedDirectories = true;
            ShowIlSequencePointsFlag = false;
            ReferencePaths = new List<string>();
        }

        public CSharpDecompiler GetDecompiler(string assemblyFileName)
        {
            var (config, resolver, debug, _) = PrepareForDecompile(assemblyFileName);
            return new CSharpDecompiler(assemblyFileName, resolver, config) { DebugInfoProvider = debug };
        }

        private (DecompilerSettings, UniversalAssemblyResolver, IDebugInfoProvider, PEFile)
            PrepareForDecompile(string assemblyFileName)
        {
            var mod = new PEFile(assemblyFileName);
            var meta = mod.Metadata.DetectTargetFrameworkId();
            var resolver = new UniversalAssemblyResolver(assemblyFileName, false, meta);
            foreach (var path in ReferencePaths)
            {
                resolver.AddSearchDirectory(path);
            }
            var config = GetSettings(mod);
            var debug = TryLoadPdb(mod);
            return (config, resolver, debug, mod);
        }

        private static IDebugInfoProvider TryLoadPdb(PEFile module)
        {
            var peFile = module.FileName;
            var pdbFile = Path.ChangeExtension(peFile, ".pdb");
            if (File.Exists(pdbFile))
            {
                return DebugInfoUtils.FromFile(module, pdbFile);
            }
            return DebugInfoUtils.LoadSymbols(module);
        }

        private DecompilerSettings GetSettings(PEFile module)
        {
            return new DecompilerSettings(LanguageVersion)
            {
                ThrowOnAssemblyResolveErrors = false,
                RemoveDeadCode = RemoveDeadCode,
                RemoveDeadStores = RemoveDeadStores,
                UseSdkStyleProjectFormat = WholeProjectDecompiler.CanUseSdkStyleProjectFormat(module),
                UseNestedDirectoriesForNamespaces = NestedDirectories,
            };
        }

        private void DecompileAsSolution(string outputDir, IEnumerable<string> inputAssemblies)
        {
            var projects = new List<ProjectItem>();
            foreach (var file in inputAssemblies)
            {
                var baseName = Path.GetFileNameWithoutExtension(file);
                var proj = Path.Combine(outputDir, baseName, $"{baseName}.csproj");
                var projectDir = Path.GetDirectoryName(proj);
                Directory.CreateDirectory(projectDir!);
                var pid = DecompileAsProject(file, proj);
                var item = new ProjectItem(proj, pid.PlatformName, pid.Guid, pid.TypeGuid);
                projects.Add(item);
            }
            var slnFile = $"{Path.GetFileNameWithoutExtension(outputDir)}.sln";
            SolutionCreator.WriteSolutionFile(Path.Combine(outputDir, slnFile), projects);
        }

        private ProjectId DecompileAsProject(string assemblyFileName, string projectFileName)
        {
            var (config, resolver, debug, module) = PrepareForDecompile(assemblyFileName);
            var decompiler = new WholeProjectDecompiler(config, resolver, resolver, debug);
            using var fileOut = File.OpenWrite(projectFileName);
            using var projectWriter = new StreamWriter(fileOut);
            var projectDir = Path.GetDirectoryName(projectFileName);
            return decompiler.DecompileProject(module, projectDir, projectWriter);
        }

        private void ShowIl(string assemblyFileName, TextWriter output)
        {
            var module = new PEFile(assemblyFileName);
            output.WriteLine($"// IL code: {module.Name}");
            var disassembler = new ReflectionDisassembler(new PlainTextOutput(output), CancellationToken.None)
            {
                DebugInfo = TryLoadPdb(module),
                ShowSequencePoints = ShowIlSequencePointsFlag
            };
            disassembler.WriteModuleContents(module);
        }

        private void ListContent(string assemblyFileName, TextWriter output, ICollection<TypeKind> kinds)
        {
            var decompiler = GetDecompiler(assemblyFileName);
            foreach (var type in decompiler.TypeSystem.MainModule.TypeDefinitions)
            {
                if (!kinds.Contains(type.Kind))
                    continue;
                output.WriteLine($"{type.Kind} {type.FullName}");
            }
        }

        private void Decompile(string assemblyFileName, TextWriter output, string typeName = null)
        {
            var decompiler = GetDecompiler(assemblyFileName);
            if (typeName == null)
            {
                output.Write(decompiler.DecompileWholeModuleAsString());
                return;
            }
            var name = new FullTypeName(typeName);
            output.Write(decompiler.DecompileTypeAsString(name));
        }

        private string DecompileAsType(string assemblyFileName, string typeName)
        {
            var fqn = new FullTypeName(typeName);
            var decompiler = GetDecompiler(assemblyFileName);
            var code = decompiler.DecompileTypeAsString(fqn);
            return code;
        }

        private string DecompileAsMethod(string assemblyFileName, string typeName)
        {
            var fqn = new FullTypeName(typeName);
            var decompiler = GetDecompiler(assemblyFileName);
            var system = decompiler.TypeSystem;
            var typeInfo = system.FindType(fqn).GetDefinition()!;
            var method = typeInfo.Methods.First();
            var token = method.MetadataToken;
            var code = decompiler.DecompileAsString(token);
            return code;
        }
    }
}