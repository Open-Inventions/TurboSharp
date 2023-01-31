using System.Reflection;

namespace TurboRepo.Nuget
{
    public static class Externals
    {
        public static Assembly LoadByName(string name)
            => LoadByName(new AssemblyName(name));

        public static Assembly LoadByName(AssemblyName name)
            => Assembly.Load(name);
    }
}