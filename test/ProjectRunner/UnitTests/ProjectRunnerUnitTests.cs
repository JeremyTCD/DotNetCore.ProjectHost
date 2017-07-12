using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace JeremyTCD.ProjectRunner.Tests.UnitTests
{
    public class ProjectRunnerUnitTests
    {
        private MockRepository _mockRepository { get; }

        public ProjectRunnerUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void RunMainMethod_ThrowsExceptionIfAssemblyDoesNotHaveSpecifiedEntryClass()
        {
            // Arrange
            string testEntryClassName = "testEntryClassName";
            string testAssemblyName = "testAssemblyName";

            Mock<ILoggingService<ProjectRunner>> mockProjectRunnerLS = _mockRepository.Create<ILoggingService<ProjectRunner>>();
            mockProjectRunnerLS.Setup(p => p.IsEnabled(LogLevel.Debug)).Returns(false);

            AssemblyName stubAssemblyName = new AssemblyName(testAssemblyName);
            Mock<Assembly> mockAssembly = _mockRepository.Create<Assembly>();
            mockAssembly.Setup(a => a.GetType(testEntryClassName)).Returns((Type)null);
            mockAssembly.Setup(a => a.GetName()).Returns(stubAssemblyName);

            ProjectRunner projectRunner = CreateProjectRunner(mockProjectRunnerLS.Object);

            // Act and Assert
            Exception result = Assert.Throws<Exception>(() => projectRunner.RunMainMethod(mockAssembly.Object, testEntryClassName, null));
            _mockRepository.VerifyAll();
            Assert.Equal(string.Format(Strings.Exception_AssemblyDoesNotHaveClass, testAssemblyName, testEntryClassName), result.Message);
        }

        [Fact]
        public void RunMainMethod_ThrowsExceptionIfEntryClassDoesNotHaveMainMethod()
        {
            // Arrange
            string testEntryClassName = "testEntryClassName";
            string testAssemblyName = "testAssemblyName";

            Mock<ILoggingService<ProjectRunner>> mockProjectRunnerLS = _mockRepository.Create<ILoggingService<ProjectRunner>>();
            mockProjectRunnerLS.Setup(p => p.IsEnabled(LogLevel.Debug)).Returns(false);

            Mock<Type> mockType = _mockRepository.Create<Type>();
            AssemblyName stubAssemblyName = new AssemblyName(testAssemblyName);
            Mock<Assembly> mockAssembly = _mockRepository.Create<Assembly>();
            mockAssembly.Setup(a => a.GetType(testEntryClassName)).Returns(mockType.Object);
            mockAssembly.Setup(a => a.GetName()).Returns(stubAssemblyName);

            Mock<ITypeService> mockTypeService = _mockRepository.Create<ITypeService>();
            mockTypeService.
                Setup(t => t.GetMethod(mockType.Object, "Main", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)).
                Returns((MethodInfo)null);

            ProjectRunner projectRunner = CreateProjectRunner(mockProjectRunnerLS.Object, typeService: mockTypeService.Object);

            // Act and Assert
            Exception result = Assert.Throws<Exception>(() => projectRunner.RunMainMethod(mockAssembly.Object, testEntryClassName, null));
            Assert.Equal(string.Format(Strings.Exception_ClassDoesNotHaveMainMethod, testEntryClassName, testAssemblyName), result.Message);
        }

        private ProjectRunner CreateProjectRunner(ILoggingService<ProjectRunner> loggingService = null, IPathService pathService = null, 
            IMSBuildService msBuildService = null, IActivatorService activatorService = null, IDirectoryService directoryService = null,
            ITypeService typeService = null, IAssemblyLoadContextFactory assemblyLoadContextFactory = null)
        {
            return new ProjectRunner(loggingService, pathService, msBuildService, activatorService, directoryService, typeService,
                assemblyLoadContextFactory);
        }
    }
}
