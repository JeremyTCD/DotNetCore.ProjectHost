﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JeremyTCD.ProjectRunner {
    using System;
    using System.Reflection;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("JeremyTCD.ProjectRunner.Strings", typeof(Strings).GetTypeInfo().Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Assembly &quot;{0}&quot; does not have a class named &quot;{1}&quot;.
        /// </summary>
        public static string Exception_AssemblyDoesNotHaveClass {
            get {
                return ResourceManager.GetString("Exception_AssemblyDoesNotHaveClass", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Class &quot;{0}&quot; in Assembly &quot;{1}&quot; does not contain a Main method (entry point).
        /// </summary>
        public static string Exception_ClassDoesNotHaveMainMethod {
            get {
                return ResourceManager.GetString("Exception_ClassDoesNotHaveMainMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loading assembly &quot;{0}&quot; into an AssemblyLoadContext that resolves from directory &quot;{1}&quot;.
        /// </summary>
        public static string Log_LoadingAssembly {
            get {
                return ResourceManager.GetString("Log_LoadingAssembly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Publishing &quot;{0}&quot; build of project &quot;{1}&quot; to directory &quot;{2}&quot;.
        /// </summary>
        public static string Log_PublishingProject {
            get {
                return ResourceManager.GetString("Log_PublishingProject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Running Main method from class &quot;{0}&quot; of assembly &quot;{1}&quot; with args &quot;{2}&quot;.
        /// </summary>
        public static string Log_RunningMainMethod {
            get {
                return ResourceManager.GetString("Log_RunningMainMethod", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Running project &quot;{0}&quot; with arguments &quot;{1}&quot;.
        /// </summary>
        public static string Log_RunningProject {
            get {
                return ResourceManager.GetString("Log_RunningProject", resourceCulture);
            }
        }
    }
}
