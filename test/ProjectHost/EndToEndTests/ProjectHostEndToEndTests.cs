using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.DotNetCore.ProjectHost.Tests.EndToEndTests
{
    public class ProjectHostEndToEndTests
    {
        private MockRepository _mockRepository { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(ProjectHostEndToEndTests)}Temp");
        private DirectoryService _directoryService { get; }

        public ProjectHostEndToEndTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            Mock<ILoggingService<DirectoryService>> mockDSLS = _mockRepository.Create<ILoggingService<DirectoryService>>();
            _directoryService = new DirectoryService(mockDSLS.Object);
        }

        /// <summary>
        /// Ensures that methods from assembly loaded in a <see cref="DirectoryAssemblyLoadContext"/> context by <see cref="ProjectLoader.Load(string, string, string)"/> 
        /// can be run by <see cref="MethodRunner.Run(Assembly, string, string, string[])"/>. Also ensures that <see cref="ProjectHostServiceCollectionExtensions"/> loads
        /// correct services.
        /// </summary>
        /// <param name="projectDir"></param>
        [Theory]
        [MemberData(nameof(RunRunsEntryMethodData))]
        public void Run_RunsEntryMethod(string projectDir)
        {
            // Arrange
            string tempDir = $"{_tempDir}{projectDir}";
            _directoryService.Empty(tempDir); // TODO for netstandard2.0 and earlier, AssemblyLoadContexts cannot be unloaded (can't reuse same directory within same process)
            string solutionDir = Path.GetFullPath(typeof(ProjectHostEndToEndTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string projectName = projectDir;
            string projectAbsSrcDir = $"{solutionDir}/test/{projectDir}";
            string projectAbsFilePath = $"{tempDir}/{projectName}.csproj";
            string assemblyName = projectName;
            string className = $"{projectName}.EntryStubClass";
            int testExitCode = 10; // Arbitrary 
            string[] stubArgs = new string[] { testExitCode.ToString() };

            _directoryService.Copy(projectAbsSrcDir, tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });

            IServiceCollection services = new ServiceCollection();
            services.AddProjectHost();
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            IProjectLoader loader = serviceProvider.GetService<IProjectLoader>();
            IMethodRunner runner = serviceProvider.GetService<IMethodRunner>();

            // Act
            Assembly assembly = loader.Load(projectAbsFilePath, assemblyName);
            int result = runner.Run(assembly, className, args: stubArgs);

            // Assert
            Assert.Equal(testExitCode, result);
            (serviceProvider as IDisposable).Dispose();
        }

        public static IEnumerable<object[]> RunRunsEntryMethodData()
        {
            yield return new object[] { "StubProject.NewerFramework" };
            yield return new object[] { "StubProject.OlderFramework" };
        }
    }
}
