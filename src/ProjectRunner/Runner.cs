using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace JeremyTCD.ProjectRunner
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
        /// <param name="entryClassName"></param>
        /// <param name="entryMethodName"></param>
        /// <returns>
        /// Integer return value of entry method or null if entry method returns void
        /// </returns>
        public virtual int? Run(string projFile, string entryAssemblyName, string entryClassName = "Program", string entryMethodName = "Main", string[] args = null)
        {
            if (_loggingService.IsEnabled(LogLevel.Information))
            {
                _loggingService.LogInformation(Strings.Log_RunningProject, projFile, String.Join(",", args));
            }

            // TODO if already built (plugin folder etc) just need outDir, proj file not relevant
            string absProjFilePath = _pathService.GetAbsolutePath(projFile);
            string directory = _directoryService.GetParent(absProjFilePath).FullName;
            string outDir = $"{directory}/bin/publish";
            string entryAssemblyFilePath = $"{outDir}/{entryAssemblyName}.dll";

            // Publish project
            PublishProject(absProjFilePath, outDir);

            // Load entry assembly
            Assembly entryAssembly = LoadEntryAssembly(outDir, entryAssemblyFilePath);

            // Run entry method
            object result = RunEntryMethod(entryAssembly, entryClassName, entryMethodName, args);

            return result as int?;
        }

        // TODO should be internal or private but testable in isolation
        public void PublishProject(string absProjFilePath, string outDir)
        {
            // Only need to build for 1 framework
            string targetFramework = _msBuildService.GetTargetFrameworks(absProjFilePath).First();

            _loggingService.LogDebug(Strings.Log_PublishingProject, targetFramework, absProjFilePath, outDir);

            _msBuildService.Build(absProjFilePath, $"/t:restore,publish /p:outdir={outDir},configuration=release,targetframework={targetFramework}");
        }

        // TODO should be internal or private but testable in isolation
        public Assembly LoadEntryAssembly(string outDir, string entryAssemblyFilePath)
        {
            _loggingService.LogDebug(Strings.Log_LoadingAssembly, entryAssemblyFilePath, outDir);

            AssemblyLoadContext alc = _assemblyLoadContextFactory.CreateAssemblyLoadContext(outDir);

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
