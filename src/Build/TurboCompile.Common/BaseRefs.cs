using System.Collections.Generic;
using TurboCompile.API.External;

namespace TurboCompile.Common
{
    public static class BaseRefs
    {
        public static HashSet<IExternalRef> CreateStdSet()
        {
            var set = new HashSet<IExternalRef>
            {
                new NameRef("System.Collections"),
                new NameRef("System.Xml.ReaderWriter"),
                new NameRef("System.Xml.XmlSerializer")
            };
            return set;
        }
    }
}