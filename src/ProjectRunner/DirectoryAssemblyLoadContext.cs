using System;
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
        private Dictionary<string, string> _assemblyFiles { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public DirectoryAssemblyLoadContext(string directory)
        {
            _directory = directory;
            // TODO how to handle runtime specific dlls?
            string[] assemblyFiles = Directory.GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);
            foreach (string assemblyFile in assemblyFiles)
            {
                AssemblyName name = GetAssemblyName(assemblyFile);
                _assemblyFiles.Add(name.Name, assemblyFile);
            }
            Resolving += DirectoryResolver;
        }

        private Assembly DirectoryResolver(AssemblyLoadContext loadContext, AssemblyName name)
        {
            // Ignore resources dlls
            if (name.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            string assemblyFile = _assemblyFiles[name.Name];
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
