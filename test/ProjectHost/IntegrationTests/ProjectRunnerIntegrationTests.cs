using JeremyTCD.DotNetCore.Utils;
using Moq;
using StructureMap;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.DotNetCore.ProjectHost.Tests.IntegrationTests
{
    public class ProjectRunnerIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(ProjectRunnerIntegrationTests)}Temp");
        private DirectoryService _directoryService { get; }

        public ProjectRunnerIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            Mock<ILoggingService<DirectoryService>> mockDSLS = _mockRepository.Create<ILoggingService<DirectoryService>>();
            _directoryService = new DirectoryService(mockDSLS.Object);
        }

        [Theory]
        [MemberData(nameof(RunRunsEntryMethodData))]
        public void Run_RunsEntryMethod(string projectDir)
        {
            // Arrange
            string tempDir = $"{_tempDir}.{projectDir}"; // TODO netstandard2.0 and earlier, AssemblyLoadContexts cannot be unloaded
            _directoryService.DeleteIfExists(tempDir, true);
            _directoryService.Create(tempDir);
            _directoryService.SetCurrentDirectory(tempDir);
            string solutionDir = Path.GetFullPath(typeof(ProjectRunnerIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../../");
            string projectName = projectDir;
            string projectAbsSrcDir = $"{solutionDir}test/{projectDir}";
            string projectAbsDestDir = $"{tempDir}/{projectDir}";
            string projectAbsFilePath = $"{tempDir}/{projectDir}/{projectName}.csproj";
            string entryAssemblyName = projectName;
            string entryClassName = $"{projectName}.EntryStubClass";
            int testExitCode = 10; // Arbitrary 
            string[] stubArgs = new string[] { testExitCode.ToString() };

            _directoryService.Copy(projectAbsSrcDir, projectAbsDestDir, excludePatterns: new string[] { "^bin$", "^obj$" });

            IContainer container = new Container(new ProjectHostRegistry());
            ProjectRunner runner = container.GetInstance<ProjectRunner>();

            // Act
            int result = runner.Run(projectAbsFilePath, entryAssemblyName, entryClassName, publishConfiguration: "Debug", args: stubArgs);

            // Assert
            Assert.Equal(testExitCode, result);
            container.Dispose();
        }

        public static IEnumerable<object[]> RunRunsEntryMethodData()
        {
            yield return new object[] { "StubProject.NewerFramework" };
            yield return new object[] { "StubProject.OlderFramework" };
        }
    }
}
