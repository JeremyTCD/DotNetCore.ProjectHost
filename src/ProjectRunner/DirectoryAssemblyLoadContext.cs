using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace JeremyTCD.ProjectRunner
{
    /// <summary>
    /// Resolves assemblies from specified directory.
    /// </summary>
    public class DirectoryAssemblyLoadContext : AssemblyLoadContext
    {
        private string _directory { get; }
        private Dictionary<string, string> _assemblyFiles { get; }

        public DirectoryAssemblyLoadContext(string directory)
        {
            _directory = directory;
            string[] assemblyFiles = Directory.GetFiles(directory, "*.dll", SearchOption.AllDirectories);
            foreach (string assemblyFile in assemblyFiles)
            {
                AssemblyName name = GetAssemblyName(assemblyFile);
                _assemblyFiles.Add(name.FullName, assemblyFile);
            }
            Resolving += DirectoryResolver;
        }

        private Assembly DirectoryResolver(AssemblyLoadContext loadContext, AssemblyName name)
        {
            string assemblyFile = _assemblyFiles[name.FullName];
            try
            {
                return loadContext.LoadFromAssemblyPath(assemblyFile);
            }
            catch
            {
                // Swallow exceptions so other Resolving subscribers can have a go
                return null;
            }

        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Attempt to copy assembly from default ALC if it has already been loaded
            return null;
        }
    }
}
