using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Xunit;

namespace JeremyTCD.DotNetCore.ProjectHost.Tests.IntegrationTests
{
    public class ProjectHostServiceCollectionExtensionsIntegrationTests
    {
        [Fact]
        public void AddProjectHost_ConfiguresServicesCorrectly()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();

            // Act
            services.AddProjectHost();

            // Assert
            ServiceDescriptorComparer comparer = new ServiceDescriptorComparer();
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IMethodRunner, MethodRunner>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IProjectLoader, ProjectLoader>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton<IAssemblyLoadContextFactory, DefaultAssemblyLoadContextFactory>(), comparer));
        }
    }
}
