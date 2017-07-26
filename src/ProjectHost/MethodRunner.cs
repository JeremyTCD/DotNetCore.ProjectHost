using JeremyTCD.DotNetCore.Utils;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace JeremyTCD.DotNetCore.ProjectHost
{
    public class MethodRunner : IMethodRunner
    {
        private ILoggingService<MethodRunner> _loggingService { get; }
        private IActivatorService _activatorService { get; }
        private ITypeService _typeService { get; }

        public MethodRunner(ILoggingService<MethodRunner> loggingService, IActivatorService activatorService, ITypeService typeService)
        {
            _activatorService = activatorService;
            _loggingService = loggingService;
            _typeService = typeService;
        }

        /// <summary>
        /// Instantiates instance of class <paramref name="className"/> and calls method <paramref name="methodName"/> with args 
        /// <paramref name="args"/>.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="args"></param> 
        /// <param name="className">Full name (inclusive of namespace)</param>
        /// <param name="methodName"></param>
        /// <returns>
        /// Integer return value of method or null if method returns void or a non int object
        /// </returns>
        public virtual int Run(Assembly assembly, string className, string methodName = "Main", string[] args = null)
        {
            if (_loggingService.IsEnabled(LogLevel.Debug))
            {
                _loggingService.LogDebug(Strings.Log_RunningMethod, methodName, className, assembly.GetName().Name, String.Join(",", args));
            }

            Type entryType = assembly.GetType(className);
            if (entryType == null)
            {
                throw new Exception(string.Format(Strings.Exception_AssemblyDoesNotHaveClass, assembly.GetName().Name, className));
            }

            MethodInfo entryMethod = _typeService.GetMethod(entryType, methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
            if (entryMethod == null)
            {
                throw new Exception(string.Format(Strings.Exception_ClassDoesNotHaveEntryMethod, className, assembly.GetName().Name, methodName));
            }

            Object entryObject = _activatorService.CreateInstance(entryType);

            int? result = entryMethod.Invoke(entryObject, new object[] { args }) as int?;

            return result ?? 0;
        }
    }
}
