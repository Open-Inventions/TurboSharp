using System.Collections.Generic;
using System.IO;
using System.Linq;
using TurboBase.IO;
using TurboMeta.API.File;
using TurboMeta.API.Proj;
using TurboMeta.API.Sol;
using static TurboMeta.Common.Sol.SolutionLoader;

namespace TurboMeta.Common.Proj
{
    public abstract class ProjectLoader : BaseFileLoader
    {
        protected abstract string CodeExtension { get; }

        protected override ISolution SafeLoad(string path,
            IFileLoader<ISolution> parent = null)
        {
            var proj = new Project(path, $"*{CodeExtension}");
            var projects = new HashSet<IProject>();
            FindRef(proj, projects, parent ?? this);

            var fake = Path.ChangeExtension(proj.FilePath, SolExt);
            var sol = new MemSolution(fake, projects.ToArray());
            return sol;
        }

        private static void FindRef(IProject proj, ISet<IProject> projects,
            IFileLoader<ISolution> parent)
        {
            projects.Add(proj);

            var owner = proj.FilePath;
            foreach (var pRef in proj.ProjectReferences)
            {
                var path = IoTools.GetAbsPath(pRef.FilePath, owner);
                var current = parent?.Load(path, parent)
                    .ProjectsInOrder.First();
                FindRef(current, projects, parent);
            }
        }
    }
}