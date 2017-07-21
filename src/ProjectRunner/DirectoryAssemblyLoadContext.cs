using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            IEnumerable<string> files = Directory.
                GetFiles(directory, "*.dll", SearchOption.TopDirectoryOnly);

            // Always prefer native images - https://github.com/dotnet/coreclr/blob/40b25c8955a00e47d2b061f2f795ffc917ccca1a/Documentation/building/crossgen.md
            IEnumerable<string> filesWithNativeImages = files.
                Where(f => f.EndsWith(".ni.dll")).
                Select(f => f.Replace(".ni", ""));

            foreach (string file in files)
            {
                if (filesWithNativeImages.Contains(file))
                {
                    continue;
                }

                string name;
                try
                {
                    name = GetAssemblyName(file).Name; // Assemblies name isn't necessarily the same as its file name
                }
                catch (BadImageFormatException)
                {
                    name = Path.GetFileNameWithoutExtension(file);
                }

                _assemblyFiles.Add(name, file);
            }
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            // Ignore resources dlls
            if (assemblyName.Name.EndsWith(".resources", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (!_assemblyFiles.TryGetValue(assemblyName.Name, out string assemblyFile))
            {
                // Let default context have a go
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
