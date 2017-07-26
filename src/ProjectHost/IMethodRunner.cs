using System.Reflection;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public interface IMethodRunner
    {
        int Run(Assembly assembly, string className, string methodName = "Main", string[] args = null);
    }
}
