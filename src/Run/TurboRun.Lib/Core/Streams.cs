using System;
using System.IO;

namespace TurboRun.Core
{
    public record Streams(
        TextReader In,
        TextWriter Out,
        TextWriter Error
    )
    {
        public void Load()
        {
            Console.SetIn(In);
            Console.SetOut(Out);
            Console.SetError(Error);
        }

        public static Streams Save()
        {
            return new Streams(Console.In, Console.Out, Console.Error);
        }
    }
}