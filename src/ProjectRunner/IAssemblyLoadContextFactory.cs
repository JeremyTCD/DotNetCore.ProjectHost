using System.Runtime.Loader;

namespace JeremyTCD.ProjectRunner
{
    public interface IAssemblyLoadContextFactory
    {
        AssemblyLoadContext CreateAssemblyLoadContext(string directory);
    }
}
