using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using StructureMap;

namespace JeremyTCD.ProjectRunner
{
    public class ProjectRunnerRegistry : Registry
    {
        public ProjectRunnerRegistry()
        {
            IServiceCollection services = new ServiceCollection();
            services.
                AddLogging();
            this.Populate(services);

            For<IPathService>().Singleton().Use<PathService>();
            For<IDirectoryService>().Singleton().Use<DirectoryService>();
            For<IMSBuildService>().Singleton().Use<MSBuildService>();
            For<IActivatorService>().Singleton().Use<ActivatorService>();
            For(typeof(ILoggingService<>)).Singleton().Use(typeof(LoggingService<>));
            For<IProcessService>().Singleton().Use<ProcessService>();
            For<ITypeService>().Singleton().Use<TypeService>();

            For<Runner>().Singleton().Use<Runner>();
            For<IAssemblyLoadContextFactory>().Singleton().Use<DefaultAssemblyLoadContextFactory>();
        }
    }
}
