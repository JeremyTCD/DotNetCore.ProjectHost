using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
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

            Assert.True(services.Contains(ServiceDescriptor.Singleton<ILoggerFactory, LoggerFactory>(), comparer));
            Assert.True(services.Contains(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(Logger<>)), comparer));
        }

        [Fact]
        public void AddProjectHost_GeneratedServiceProviderDisposesCorrectly()
        {
            // Arrange
            ServiceCollection services = new ServiceCollection();
            services.AddProjectHost();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();

            // Act
            (serviceProvider as IDisposable).Dispose();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => loggerFactory.AddProvider(null));
        }
    }
}
