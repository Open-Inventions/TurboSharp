﻿using System.Linq;
using System.Reflection;
using TurboCompile.API.External;

namespace TurboCompile.Common
{
    public static class Externals
    {
        public static NuGetRef[] Parse(string prefix, string text)
        {
            var lines = text.Split('\n')
                .TakeWhile(l => l.StartsWith(prefix))
                .Select(l => Parse(l[prefix.Length..]))
                .ToArray();
            return lines;
        }

        private const string NugetPrefix = "nuget:";

        private static NuGetRef Parse(string line)
        {
            var term = line.Trim().Trim('"');
            if (term.StartsWith(NugetPrefix))
            {
                var nugetTerm = term[NugetPrefix.Length..];
                var parts = nugetTerm.Split(',', 2);
                var pkgName = parts.First().Trim();
                var pkgVer = parts.Skip(1).First().Trim();
                return new NuGetRef(pkgName, pkgVer);
            }
            return null;
        }

        public static Assembly LoadByName(string name)
            => LoadByName(new AssemblyName(name));

        public static Assembly LoadByName(AssemblyName name)
            => Assembly.Load(name);
    }
}