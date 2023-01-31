using System;
using System.Collections.Generic;
using System.Linq;

namespace TurboCompile.Common
{
    public sealed class CompileError : Exception
    {
        public ICollection<(string, string)> Errors { get; }

        public CompileError(ICollection<(string, string)> errors) : base(ToMessage(errors))
        {
            Errors = errors;
        }

        private static string ToMessage(IEnumerable<(string, string)> errors)
            => string.Join(Environment.NewLine, errors.Select(e => $"{e.Item1}: {e.Item2}"));
    }
}