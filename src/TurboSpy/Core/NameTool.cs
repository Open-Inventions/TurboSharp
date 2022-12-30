using System;
using System.Linq;
using ICSharpCode.Decompiler.TypeSystem;

namespace TurboSpy.Core
{
    public static class NameTool
    {
        public static string ToSimple(this IType type)
        {
            var known = (type as ITypeDefinition)?.KnownTypeCode
                        ?? KnownTypeCode.None;
            var code = type.GetTypeCode();
            var full = type.FullName;
            if (code == TypeCode.Empty && known == KnownTypeCode.None)
            {
                var prm = string.Join(",", type.TypeArguments
                    .Select(t => t.ToSimple()));
                return prm.Length == 0 ? type.Name : $"{type.Name}<{prm}>";
            }
            switch (full)
            {
                case "System.Boolean": return "bool";
                case "System.Byte": return "byte";
                case "System.Char": return "char";
                case "System.Int32": return "int";
                case "System.Single": return "float";
                case "System.Void": return "void";
                case "System.String": return "string";
                case "System.Object": return "object";
                default: throw new InvalidOperationException(full);
            }
        }
    }
}