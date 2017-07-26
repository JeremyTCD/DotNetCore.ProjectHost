using JeremyTCD.DotNetCore.Utils;
using Microsoft.DotNet.PlatformAbstractions;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public class ProjectLoader : IProjectLoader
    {
        private ILoggingService<MethodRunner> _loggingService { get; }
        private IPathService _pathService { get; }
        private IMSBuildService _msBuildService { get; }
        private IDirectoryService _directoryService { get; }
        private IAssemblyLoadContextFactory _assemblyLoadContextFactory { get; }

        public ProjectLoader(ILoggingService<MethodRunner> loggingService, IPathService pathService, IMSBuildService msbuildService,
            IDirectoryService directoryService, IAssemblyLoadContextFactory assemblyLoadContextFactory)
        {
            _loggingService = loggingService;
            _pathService = pathService;
            _msBuildService = msbuildService;
            _directoryService = directoryService;
            _assemblyLoadContextFactory = assemblyLoadContextFactory;
        }

        public virtual Assembly Load(string projFile, string entryAssemblyName, string buildConfiguration = "Release")
        {
            _loggingService.LogDebug(Strings.Log_LoadingProject, projFile, entryAssemblyName, buildConfiguration);

            string absProjFilePath = _pathService.GetAbsolutePath(projFile);
            string rid = RuntimeEnvironment.GetRuntimeIdentifier();
            string projFileDirectory = _directoryService.GetParent(absProjFilePath).FullName;
            string targetFramework = _msBuildService.GetTargetFrameworks(absProjFilePath).First();
            string publishDirectory = $"{projFileDirectory}/bin/{buildConfiguration}/{targetFramework}/{rid}";

            // Delete publish directory - IncrementalClean target is buggy and deletes required assemblies if directory isn't empty - https://github.com/Microsoft/msbuild/issues/1054
            _directoryService.DeleteIfExists(publishDirectory, true);

            // Build project
            // TODO already built case
            BuildProject(absProjFilePath, buildConfiguration, rid);

            // Load entry assembly
            return LoadEntryAssembly(publishDirectory, entryAssemblyName);
        }

        // TODO should be internal or private but testable in isolation
        public void BuildProject(string absProjFilePath, string publishConfiguration, string rid)
        {
            _loggingService.LogDebug(Strings.Log_BuildingProject, absProjFilePath);

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
    }
}
