using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace JeremyTCD.ProjectRunner.Tests.UnitTests
{
    public class RunnerUnitTests
    {
        private MockRepository _mockRepository { get; }

        public RunnerUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void RunEntryMethod_ThrowsExceptionIfAssemblyDoesNotHaveSpecifiedEntryClass()
        {
            // Arrange
            string testEntryClassName = "testEntryClassName";
            string testAssemblyName = "testAssemblyName";

            Mock<ILoggingService<Runner>> mockRunnerLS = _mockRepository.Create<ILoggingService<Runner>>();
            mockRunnerLS.Setup(p => p.IsEnabled(LogLevel.Debug)).Returns(false);

            AssemblyName stubAssemblyName = new AssemblyName(testAssemblyName);
            Mock<Assembly> mockAssembly = _mockRepository.Create<Assembly>();
            mockAssembly.Setup(a => a.GetType(testEntryClassName)).Returns((Type)null);
            mockAssembly.Setup(a => a.GetName()).Returns(stubAssemblyName);

            Runner runner = CreateRunner(mockRunnerLS.Object);

            // Act and Assert
            Exception result = Assert.Throws<Exception>(() => runner.RunEntryMethod(mockAssembly.Object, testEntryClassName, null, null));
            _mockRepository.VerifyAll();
            Assert.Equal(string.Format(Strings.Exception_AssemblyDoesNotHaveClass, testAssemblyName, testEntryClassName), result.Message);
        }

        [Fact]
        public void RunEntryMethod_ThrowsExceptionIfEntryClassDoesNotHaveEntryMethod()
        {
            // Arrange
            string testEntryClassName = "testEntryClassName";
            string testAssemblyName = "testAssemblyName";
            string testEntryMethodName = "testEntryMethodName";

            Mock<ILoggingService<Runner>> mockRunnerLS = _mockRepository.Create<ILoggingService<Runner>>();
            mockRunnerLS.Setup(p => p.IsEnabled(LogLevel.Debug)).Returns(false);

            Mock<Type> mockType = _mockRepository.Create<Type>();
            AssemblyName stubAssemblyName = new AssemblyName(testAssemblyName);
            Mock<Assembly> mockAssembly = _mockRepository.Create<Assembly>();
            mockAssembly.Setup(a => a.GetType(testEntryClassName)).Returns(mockType.Object);
            mockAssembly.Setup(a => a.GetName()).Returns(stubAssemblyName);

            Mock<ITypeService> mockTypeService = _mockRepository.Create<ITypeService>();
            mockTypeService.
                Setup(t => t.GetMethod(mockType.Object, testEntryMethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)).
                Returns((MethodInfo)null);

            Runner runner = CreateRunner(mockRunnerLS.Object, typeService: mockTypeService.Object);

            // Act and Assert
            Exception result = Assert.Throws<Exception>(() => runner.RunEntryMethod(mockAssembly.Object, testEntryClassName, testEntryMethodName, null));
            Assert.Equal(string.Format(Strings.Exception_ClassDoesNotHaveEntryMethod, testEntryClassName, testAssemblyName, testEntryMethodName), result.Message);
        }

        private Runner CreateRunner(ILoggingService<Runner> loggingService = null, IPathService pathService = null, 
            IMSBuildService msBuildService = null, IActivatorService activatorService = null, IDirectoryService directoryService = null,
            ITypeService typeService = null, IAssemblyLoadContextFactory assemblyLoadContextFactory = null)
        {
            return new Runner(loggingService, pathService, msBuildService, activatorService, directoryService, typeService,
                assemblyLoadContextFactory);
        }
    }
}
