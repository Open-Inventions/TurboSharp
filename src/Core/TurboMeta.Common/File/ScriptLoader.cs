using System.IO;
using TurboMeta.API.File;
using TurboMeta.API.Proj;
using TurboMeta.API.Sol;
using static TurboMeta.Common.Sol.SolutionLoader;

namespace TurboMeta.Common.File
{
    public abstract class ScriptLoader : BaseFileLoader
    {
        protected override ISolution SafeLoad(string path,
            IFileLoader<ISolution> parent = null)
        {
            var proj = LoadScript(path);
            var fake = Path.ChangeExtension(proj.FilePath, SolExt);
            var sol = new MemSolution(fake, proj);
            return sol;
        }

        protected abstract IProject LoadScript(string path);
    }
}