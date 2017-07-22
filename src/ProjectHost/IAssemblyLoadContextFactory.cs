using System.Runtime.Loader;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public interface IAssemblyLoadContextFactory
    {
        AssemblyLoadContext CreateAssemblyLoadContext(string directory);
    }
}
