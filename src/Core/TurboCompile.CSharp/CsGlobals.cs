using System;
using System.Text;
using TurboCompile.API;

namespace TurboCompile.CSharp
{
    public record CsGlobals(
        string FxVer = ".NETCoreApp,Version=v6.0",
        string Company = "Demo",
        string Product = "Demo",
        string Title = "Demo",
        string Mode = "Debug",
        Version Ver = default
    )
    {
        public string Generate()
        {
            var bld = new StringBuilder();
            bld.AppendLine();
            bld.AppendLine("global using global::System;");
            bld.AppendLine("global using global::System.Collections.Generic;");
            bld.AppendLine("global using global::System.IO;");
            bld.AppendLine("global using global::System.Linq;");
            bld.AppendLine("global using global::System.Net.Http;");
            bld.AppendLine("global using global::System.Threading;");
            bld.AppendLine("global using global::System.Threading.Tasks;");
            bld.AppendLine();
            bld.AppendLine("[assembly: global::System.Runtime.Versioning.Target" +
                           $"FrameworkAttribute(\"{FxVer}\", FrameworkDisplayName = \"\")]");
            bld.AppendLine();
            bld.AppendLine($"[assembly: System.Reflection.AssemblyCompanyAttribute(\"{Company}\")]");
            bld.AppendLine($"[assembly: System.Reflection.AssemblyConfigurationAttribute(\"{Mode}\")]");
            bld.AppendLine($"[assembly: System.Reflection.AssemblyFileVersionAttribute(\"{Ver}\")]");
            bld.AppendLine($"[assembly: System.Reflection.Assembly" +
                           $"InformationalVersionAttribute(\"{Ver.ToString(3)}\")]");
            bld.AppendLine($"[assembly: System.Reflection.AssemblyProductAttribute(\"{Product}\")]");
            bld.AppendLine($"[assembly: System.Reflection.AssemblyTitleAttribute(\"{Title}\")]");
            bld.AppendLine($"[assembly: System.Reflection.AssemblyVersionAttribute(\"{Ver}\")]");
            bld.AppendLine();
            return bld.ToString();
        }

        public CsGlobals SetNameAndVer(AssemblyMeta meta)
            => this with
            {
                Ver = Version.Parse(meta.Version),
                Company = meta.Company ?? meta.Name,
                Product = meta.Product ?? meta.Name,
                Title = meta.Title ?? meta.Name
            };
    }
}