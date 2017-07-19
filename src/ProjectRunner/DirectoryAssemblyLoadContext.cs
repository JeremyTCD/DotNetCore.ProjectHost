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
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Ignore resources dlls
            if (assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            try
            {
                string assemblyFile = _assemblyFiles[assemblyName.Name];

                return LoadFromAssemblyPath(assemblyFile);
            }
            catch 
            {
                // Let default context have a go
                return null;
            }
        }
    }
}
