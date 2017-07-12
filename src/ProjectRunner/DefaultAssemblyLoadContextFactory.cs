using System.Runtime.Loader;

namespace JeremyTCD.ProjectRunner
{
    public class DefaultAssemblyLoadContextFactory : IAssemblyLoadContextFactory
    {
        public AssemblyLoadContext CreateAssemblyLoadContext(string directory)
        {
            return new DirectoryAssemblyLoadContext(directory);
        }
    }
}
