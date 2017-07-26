using System.Reflection;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public interface IProjectLoader
    {
        Assembly Load(string projFile, string entryAssemblyName, string buildConfiguration = "Release");
        void BuildProject(string absProjFilePath, string publishConfiguration, string rid);
        Assembly LoadEntryAssembly(string publishDirectory, string entryAssemblyName);
    }
}
