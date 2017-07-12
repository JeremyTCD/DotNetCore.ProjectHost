using JeremyTCD.DotNetCore.Utils;
using Moq;
using StructureMap;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.ProjectRunner.Tests.IntegrationTests
{
    public class ProjectRunnerIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(ProjectRunner)}Temp");
        private DirectoryService _directoryService { get; }

        public ProjectRunnerIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            Mock<ILoggingService<DirectoryService>> mockDSLS = _mockRepository.Create<ILoggingService<DirectoryService>>();
            _directoryService = new DirectoryService(mockDSLS.Object);
            _directoryService.Delete(_tempDir, true);
            _directoryService.Create(_tempDir);
            _directoryService.SetCurrentDirectory(_tempDir);
        }

        [Fact]
        public void Run_RunsEntryMethod()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(ProjectRunnerIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../../");
            string projectDir = "ProjectRunner.StubProject";
            string projectName = "JeremyTCD.ProjectRunner.Tests.StubProject";
            string projectAbsSrcDir = $"{solutionDir}test/{projectDir}";
            string projectAbsDestDir = $"{_tempDir}/{projectDir}";
            string projectAbsFilePath = $"{_tempDir}/{projectDir}/{projectName}.csproj";
            string entryAssemblyName = projectName;
            string entryClassName = $"{projectName}.StubClass";
            string[] stubArgs = new string[] { "test", "args" };

            _directoryService.Copy(projectAbsSrcDir, projectAbsDestDir, excludePatterns: new string[] { "^bin$", "^obj$" });

            IContainer container = new Container(new ProjectRunnerRegistry());
            ProjectRunner projectRunner = container.GetInstance<ProjectRunner>();

            ThreadSpecificStringWriter tssw = new ThreadSpecificStringWriter();
            Console.SetOut(tssw);

            // Act
            int? result = projectRunner.Run(projectAbsFilePath, entryAssemblyName, entryClassName, stubArgs);

            // Assert
            Assert.Equal(0, result);
            tssw.Dispose();
            string output = tssw.ToString();
            Assert.Equal(string.Join(",", stubArgs), output);
        }
    }
}
