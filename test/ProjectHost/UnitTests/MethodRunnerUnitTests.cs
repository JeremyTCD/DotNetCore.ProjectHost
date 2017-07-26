using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Reflection;
using Xunit;

namespace JeremyTCD.DotNetCore.ProjectHost.Tests.UnitTests
{
    public class MethodRunnerUnitTests
    {
        private MockRepository _mockRepository { get; }

        public MethodRunnerUnitTests()
        {
            _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
        }

        [Fact]
        public void RunEntryMethod_ThrowsExceptionIfAssemblyDoesNotHaveSpecifiedEntryClass()
        {
            // Arrange
            string testEntryClassName = "testEntryClassName";
            string testAssemblyName = "testAssemblyName";

            Mock<ILoggingService<MethodRunner>> mockRunnerLS = _mockRepository.Create<ILoggingService<MethodRunner>>();
            mockRunnerLS.Setup(p => p.IsEnabled(LogLevel.Debug)).Returns(false);

            AssemblyName stubAssemblyName = new AssemblyName(testAssemblyName);
            Mock<Assembly> mockAssembly = _mockRepository.Create<Assembly>();
            mockAssembly.Setup(a => a.GetType(testEntryClassName)).Returns((Type)null);
            mockAssembly.Setup(a => a.GetName()).Returns(stubAssemblyName);

            MethodRunner runner = CreateLoader(mockRunnerLS.Object);

            // Act and Assert
            Exception result = Assert.Throws<Exception>(() => runner.Run(mockAssembly.Object, testEntryClassName, null, null));
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

            Mock<ILoggingService<MethodRunner>> mockRunnerLS = _mockRepository.Create<ILoggingService<MethodRunner>>();
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

            MethodRunner runner = CreateLoader(mockRunnerLS.Object, typeService: mockTypeService.Object);

            // Act and Assert
            Exception result = Assert.Throws<Exception>(() => runner.Run(mockAssembly.Object, testEntryClassName, testEntryMethodName, null));
            Assert.Equal(string.Format(Strings.Exception_ClassDoesNotHaveEntryMethod, testEntryClassName, testAssemblyName, testEntryMethodName), result.Message);
        }

        private MethodRunner CreateLoader(ILoggingService<MethodRunner> loggingService = null, IActivatorService activatorService = null,
            ITypeService typeService = null)
        {
            return new MethodRunner(loggingService, activatorService, typeService);
        }
    }
}
