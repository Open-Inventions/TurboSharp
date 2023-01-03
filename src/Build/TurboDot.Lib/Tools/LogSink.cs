using System;

namespace TurboDot.Tools
{
    internal static class LogSink
    {
        public static void ShowError(string message, string prefix = "[ERROR] ")
        {
            Console.Error.WriteLine($"{prefix}{message}");
        }

        public static void Write(string txt)
        {
            Console.Out.WriteLine(txt);
        }
    }
}