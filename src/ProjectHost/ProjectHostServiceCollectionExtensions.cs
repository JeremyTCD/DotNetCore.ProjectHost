using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public static class ProjectHostServiceCollectionExtensions
    {
        public static void AddProjectHost(this IServiceCollection services)
        {
            services.AddLogging();
            services.AddUtils();

            services.TryAdd(ServiceDescriptor.Singleton<IMethodRunner, MethodRunner>());
            services.TryAdd(ServiceDescriptor.Singleton<IProjectLoader, ProjectLoader>());
            services.TryAdd(ServiceDescriptor.Singleton<IAssemblyLoadContextFactory, DefaultAssemblyLoadContextFactory>());
        }
    }
}
