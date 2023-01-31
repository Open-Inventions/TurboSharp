using System;
using System.Text;
using TurboCompile.API;

namespace TurboCompile.VBasic
{
    public record VbGlobals(
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
            bld.AppendLine("<Assembly: Global.System.Runtime.Versioning.Target" +
                           $"FrameworkAttribute(\"{FxVer}\", FrameworkDisplayName:=\"\")>");
            bld.AppendLine();
            bld.AppendLine($"<Assembly: System.Reflection.AssemblyCompanyAttribute(\"{Company}\"),  _");
            bld.AppendLine($" Assembly: System.Reflection.AssemblyConfigurationAttribute(\"{Mode}\"),  _");
            bld.AppendLine($" Assembly: System.Reflection.AssemblyFileVersionAttribute(\"{Ver}\"),  _");
            bld.AppendLine($" Assembly: System.Reflection.Assembly" +
                           $"InformationalVersionAttribute(\"{Ver?.ToString(3)}\"),  _");
            bld.AppendLine($" Assembly: System.Reflection.AssemblyProductAttribute(\"{Product}\"),  _");
            bld.AppendLine($" Assembly: System.Reflection.AssemblyTitleAttribute(\"{Title}\"),  _");
            bld.AppendLine($" Assembly: System.Reflection.AssemblyVersionAttribute(\"{Ver}\")> ");
            bld.AppendLine();
            return bld.ToString();
        }

        public VbGlobals SetNameAndVer(AssemblyMeta meta)
            => this with
            {
                Ver = Version.Parse(meta.Version),
                Company = meta.Company ?? meta.Name,
                Product = meta.Product ?? meta.Name,
                Title = meta.Title ?? meta.Name
            };
    }
}