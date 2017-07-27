using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.DotNetCore.ProjectHost.Tests.IntegrationTests
{
    public class MethodRunnerIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(MethodRunnerIntegrationTests)}Temp");
        private DirectoryService _directoryService { get; }

        public MethodRunnerIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            Mock<ILoggingService<DirectoryService>> mockDSLS = _mockRepository.Create<ILoggingService<DirectoryService>>();
            _directoryService = new DirectoryService(mockDSLS.Object);
        }

        [Fact]
        public void Run_RunsMethod()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(MethodRunnerIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string projectDir = "StubProject.OlderFramework";
            string projectName = projectDir;
            string projectAbsSrcDir = $"{solutionDir}/test/{projectDir}";
            string outputDir = $"{projectAbsSrcDir}/bin/debug/netcoreapp1.0";
            string assemblyFilePath = $"{outputDir}/{projectName}.dll";
            string entryClassName = $"{projectName}.EntryStubClass";
            int testExitCode = 10; // Arbitrary 
            string[] stubArgs = new string[] { testExitCode.ToString() };

            DirectoryAssemblyLoadContext dalc = new DirectoryAssemblyLoadContext(outputDir);
            Assembly assembly = dalc.LoadFromAssemblyPath(assemblyFilePath);

            IServiceCollection services = new ServiceCollection();
            services.AddProjectHost();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IMethodRunner runner = serviceProvider.GetService<IMethodRunner>();
            
            // Act
            int result = runner.Run(assembly, entryClassName, args: stubArgs);

            // Assert
            Assert.Equal(testExitCode, result);
            (serviceProvider as IDisposable).Dispose();
        }
    }
}
