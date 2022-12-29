using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.TypeSystem;

namespace TurboSpy.Core
{
    public record OneFile(
        CSharpDecompiler Decompiler
    )
    {
        public AssemblyName FullAssemblyName
            => new(Decompiler.TypeSystem.MainModule.FullAssemblyName);

        public IEnumerable<AssemblyName> ReferencedModules
            => Decompiler.TypeSystem.ReferencedModules
                .OrderBy(r => r.Name).Select(mod => new AssemblyName(mod.FullAssemblyName))
                .Where(modName => !modName.Name!.Equals("corlib") && !modName.Name.StartsWith("System.Private."));

        public IDictionary<string, ITypeDefinition[]> TopLevelTypeDefs
            => Decompiler.TypeSystem.MainModule.TopLevelTypeDefinitions
                .OrderBy(r => r.FullName)
                .GroupBy(n => string.IsNullOrWhiteSpace(n.Namespace) ? RootNamespace : n.Namespace)
                .ToImmutableSortedDictionary(k => k.Key,
                    v => v.ToArray());

        private const string RootNamespace = "'";

        public string GetModuleTxt()
        {
            var bld = new StringBuilder();
            bld.AppendLine();
            var main = Decompiler.TypeSystem.MainModule;
            bld.AppendLine($"// {main.PEFile.FileName}");
            bld.AppendLine($"// {FullAssemblyName}");
            bld.AppendLine();
            var attrs = Decompiler.DecompileModuleAndAssemblyAttributesToString();
            bld.AppendLine(attrs);
            bld.AppendLine();
            return bld.ToString();
        }
    }
}