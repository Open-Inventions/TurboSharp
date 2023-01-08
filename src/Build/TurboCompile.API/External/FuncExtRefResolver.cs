using System;

namespace TurboCompile.API.External
{
    public sealed class FuncExtRefResolver : IExtRefResolver
    {
        private readonly Func<IExternalRef, string> _func;

        public FuncExtRefResolver(Func<IExternalRef, string> func)
        {
            _func = func;
        }

        public string Locate(IExternalRef external)
        {
            return _func.Invoke(external);
        }
    }
}