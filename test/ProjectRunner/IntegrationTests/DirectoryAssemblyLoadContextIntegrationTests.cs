using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace JeremyTCD.ProjectRunner.Tests.IntegrationTests
{
    public class DirectoryAssemblyLoadContextIntegrationTests
    {
        [Fact]
        public void DirectoryAssemblyLoadContext_ResolvesImplicitAndExplicitLoads()
        {
            // Arrange
            string solutionDir = Path.GetFullPath(typeof(DirectoryAssemblyLoadContextIntegrationTests).GetTypeInfo().Assembly.Location + "../../../../../../..");
            string projectDir = "StubProject.Referencer";
            string projectName = projectDir;
            string rootDirectory = $"{solutionDir}/test/{projectDir}/bin/debug/netstandard1.3/";

            DirectoryAssemblyLoadContext dalc = new DirectoryAssemblyLoadContext(rootDirectory);

            // Act
            Assembly referencer = dalc.LoadFromAssemblyName(new AssemblyName("StubProject.Referencer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"));
            Type referencerStubType = referencer.GetTypes().First();
            object referencerStubTypeInstance = Activator.CreateInstance(referencerStubType);

            // Assert
            // Ensure that StubProject.Referencee.dll was loaded implicitly (dotnetcore1.1 does not expose any methods to explicitly check if an assembly 
            // is loaded)
            Assert.NotNull(referencerStubTypeInstance);
        }

        // TODO ensure that loading of culture/platform specific assemblies works
        // TODO ensure that loading of unmanaged assemblies works
    }
}
