using System.Runtime.Loader;

namespace JeremyTCD.DotNetCore.ProjectRunner
{
    public interface IAssemblyLoadContextFactory
    {
        AssemblyLoadContext CreateAssemblyLoadContext(string directory);
    }
}
