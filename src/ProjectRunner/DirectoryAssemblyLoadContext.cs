using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace JeremyTCD.DotNetCore.ProjectRunner
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
                string name = Path.GetFileNameWithoutExtension(assemblyFile);
                _assemblyFiles.Add(name, assemblyFile);
            }
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Ignore resources dlls
            if (assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Use framework assemblies from default context
            // TODO what if simple name does not match assembly file name? revert to GetAssemblyName in constructor? 
            // need some equivalent for unmanageddlls
            if (!_assemblyFiles.TryGetValue(assemblyName.Name, out string assemblyFile))
            {
                return null;
            }

            try
            {
                return LoadFromAssemblyPath(assemblyFile);
            }
            catch
            {
                // Let default context have a go
                return null;
            }
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            if(!_assemblyFiles.TryGetValue(unmanagedDllName, out string assemblyFile))
            {
                return IntPtr.Zero;
            }

            try
            {
                return LoadUnmanagedDllFromPath(assemblyFile);
            }
            catch
            {
                // Let default context have a go
                return IntPtr.Zero;
            }
        }
    }
}
