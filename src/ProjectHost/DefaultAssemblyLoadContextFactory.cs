using System.Runtime.Loader;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public class DefaultAssemblyLoadContextFactory : IAssemblyLoadContextFactory
    {
        public AssemblyLoadContext CreateAssemblyLoadContext(string directory)
        {
            return new DirectoryAssemblyLoadContext(directory);
        }
    }
}
