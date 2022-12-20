namespace TurboSharp.Roslyn.Core
{
    public static class Globals
    {
        public static readonly string Net6Usings = @"
        global using global::System; 
        global using global::System.Collections.Generic;
        global using global::System.IO;
        global using global::System.Linq;
        global using global::System.Net.Http;
        global using global::System.Threading;
        global using global::System.Threading.Tasks;
        ";

        public static readonly string Net6RtJson = @"      
        {
            ""runtimeOptions"": {
                ""tfm"": ""net6.0"",
                ""framework"": {
                    ""name"": ""Microsoft.NETCore.App"",
                    ""version"": ""6.0.0""
                }
            }
        }
        ";
    }
}