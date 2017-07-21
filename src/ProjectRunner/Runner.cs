using JeremyTCD.DotNetCore.Utils;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace JeremyTCD.DotNetCore.ProjectRunner
{
    public class Runner
    {
        private ILoggingService<Runner> _loggingService { get; }
        private IPathService _pathService { get; }
        private IMSBuildService _msBuildService { get; }
        private IDirectoryService _directoryService { get; }
        private IAssemblyLoadContextFactory _assemblyLoadContextFactory { get; }
        private IActivatorService _activatorService { get; }
        private ITypeService _typeService { get; }

        public Runner(ILoggingService<Runner> loggingService, IPathService pathService, IMSBuildService msbuildService, IActivatorService activatorService,
            IDirectoryService directoryService, ITypeService typeService, IAssemblyLoadContextFactory assemblyLoadContextFactory)
        {
            _activatorService = activatorService;
            _loggingService = loggingService;
            _pathService = pathService;
            _msBuildService = msbuildService;
            _directoryService = directoryService;
            _assemblyLoadContextFactory = assemblyLoadContextFactory;
            _typeService = typeService;
        }

        /// <summary>
        /// Restores, builds and publishes project specified by <paramref name="projFile"/>. Loads entry assembly specified by <paramref name="entryAssemblyFile"/> in an <see cref="AssemblyLoadContext"/>.
        /// Calls entry method with args <paramref name="args"/>.
        /// </summary>
        /// <param name="projFile"></param>
        /// <param name="args"></param> 
        /// <param name="entryAssemblyName"></param>
        /// <param name="entryClassName">Full name (inclusive of namespace)</param>
        /// <param name="entryMethodName"></param>
        /// <returns>
        /// Integer return value of entry method or null if entry method returns void
        /// </returns>
        public virtual int Run(string projFile, string entryAssemblyName, string entryClassName, string entryMethodName = "Main", string publishConfiguration = "Release", string[] args = null)
        {
            if (_loggingService.IsEnabled(LogLevel.Information))
            {
                _loggingService.LogInformation(Strings.Log_RunningProject, projFile, String.Join(",", args));
            }

            string absProjFilePath = _pathService.GetAbsolutePath(projFile);
            string rid = RuntimeEnvironment.GetRuntimeIdentifier();
            string projFileDirectory = _directoryService.GetParent(absProjFilePath).FullName;
            string targetFramework = _msBuildService.GetTargetFrameworks(absProjFilePath).First();
            string publishDirectory = $"{projFileDirectory}/bin/{publishConfiguration}/{targetFramework}/{rid}";

            // Delete publish directory - IncrementalClean target is buggy and deletes required assemblies if directory isn't empty - https://github.com/Microsoft/msbuild/issues/1054
            _directoryService.DeleteIfExists(publishDirectory, true);

            // Build project
            // TODO already published case
            BuildProject(absProjFilePath, publishConfiguration, rid);

            // Load entry assembly
            Assembly entryAssembly = LoadEntryAssembly(publishDirectory, entryAssemblyName);

            // Run entry method
            int? result = RunEntryMethod(entryAssembly, entryClassName, entryMethodName, args) as int?;

            return result ?? 0;
        }

        // TODO should be internal or private but testable in isolation
        public void BuildProject(string absProjFilePath, string publishConfiguration, string rid)
        {
            _loggingService.LogDebug(Strings.Log_BuildingProject, absProjFilePath);

            // TODO runtime identifier should depend on environment
            _msBuildService.
                Build(absProjFilePath, $"/t:Restore,Build /p:Configuration={publishConfiguration},RuntimeIdentifier={rid},CopyLocalLockFileAssemblies=true");
        }
         
        // TODO should be internal or private but testable in isolation
        public Assembly LoadEntryAssembly(string publishDirectory, string entryAssemblyName)
        {

            string entryAssemblyFilePath = $"{publishDirectory}/{entryAssemblyName}.dll";

            _loggingService.LogDebug(Strings.Log_LoadingAssembly, entryAssemblyFilePath, publishDirectory);

            AssemblyLoadContext alc = _assemblyLoadContextFactory.CreateAssemblyLoadContext(publishDirectory);

            return alc.LoadFromAssemblyPath(entryAssemblyFilePath);
        }

        // TODO should be internal or private but testable in isolation
        public object RunEntryMethod(Assembly entryAssembly, string entryClassName, string entryMethodName, string[] args)
        {
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningEntryMethod, entryMethodName, entryClassName, entryAssembly.GetName().Name, String.Join(",", args)); 
            }

            Type entryType = entryAssembly.GetType(entryClassName);
            if(entryType == null)
            {
                throw new Exception(string.Format(Strings.Exception_AssemblyDoesNotHaveClass, entryAssembly.GetName().Name, entryClassName));
            }

            MethodInfo entryMethod = _typeService.GetMethod(entryType, entryMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if(entryMethod == null)
            {
                throw new Exception(string.Format(Strings.Exception_ClassDoesNotHaveEntryMethod, entryClassName, entryAssembly.GetName().Name, entryMethodName));
            }

            Object entryObject = _activatorService.CreateInstance(entryType);

            return entryMethod.Invoke(entryObject, new object[] { args });
        }
    }
}
