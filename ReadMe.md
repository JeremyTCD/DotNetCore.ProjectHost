# Project Runner
## Use Case
Useful for when a class library is used as configuration for an application (typically a console application). In such cases, application logic that utilizes
the configuration project should execute in an isolated `AssemblyLoadContext`. Project runner does just that.
## Usage
Create a class library project for use as a configuration project. It must have an assembly with a static `Main` entry method. Such an assembly is typically
referenced as part of a package.

Project runner entry method:
`JeremyTCD.ProjectRunner.ProjectRunner.Run(string projFile, string entryAssemblyFile, string entryClassName, string[] args)`
Does the following:
- Restores, builds and publishes (to a folder) project specified by `projectFile`
- Creates `AssemblyLoadContext` that loads assemblies from build output folder
- Loads assembly specified by `entryAssemblyFile`
- Instantiates instance of type specified by `entryClassName`
- Invokes method `Main` in instantiated type, supplying `args` as arguments.