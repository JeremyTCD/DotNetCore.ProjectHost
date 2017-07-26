using JeremyTCD.DotNetCore.Utils;
using Moq;
using StructureMap;
using System.IO;
using System.Reflection;
using Xunit;

namespace JeremyTCD.DotNetCore.ProjectHost.Tests.IntegrationTests
{
    public class ProjectLoaderIntegrationTests
    {
        private MockRepository _mockRepository { get; }
        private string _tempDir { get; } = Path.Combine(Path.GetTempPath(), $"{nameof(ProjectLoaderIntegrationTests)}Temp");
        private DirectoryService _directoryService { get; }

        public ProjectLoaderIntegrationTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Loose) { DefaultValue = DefaultValue.Mock };

            Mock<ILoggingService<DirectoryService>> mockDSLS = _mockRepository.Create<ILoggingService<DirectoryService>>();
            _directoryService = new DirectoryService(mockDSLS.Object);
            _directoryService.Empty(_tempDir);
        }

        [Fact]
        public void Load_BuildsProjectAndLoadsAssembly()
        {
            // Arrange
            // TODO if any tests are added and still using netstandard2.0 and earlier, note that AssemblyLoadContexts cannot be unloaded
            string solutionDir = Path.GetFullPath(typeof(MethodRunnerIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string projectDir = "StubProject.OlderFramework";
            string projectName = projectDir;
            string projectAbsSrcDir = $"{solutionDir}/test/{projectDir}";
            string projectAbsFilePath = $"{_tempDir}/{projectName}.csproj";
            string entryAssemblyName = projectName;
            int testExitCode = 10; // Arbitrary 
            string[] stubArgs = new string[] { testExitCode.ToString() };

            _directoryService.Copy(projectAbsSrcDir, _tempDir, excludePatterns: new string[] { "^bin$", "^obj$" });

            IContainer container = new Container(new ProjectHostRegistry());
            ProjectLoader loader = container.GetInstance<ProjectLoader>();

            // Act
            Assembly result = loader.Load(projectAbsFilePath, entryAssemblyName);

            // Assert
            Assert.Equal(entryAssemblyName, result.GetName().Name);
            container.Dispose();
        }
    }
}
