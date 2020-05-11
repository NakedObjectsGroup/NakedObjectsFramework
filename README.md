Naked Objects Framework
=======================

This branch contains source under development for the next release. These are the principal changes:

- All assemblies will now be compiled against .NET Core 3
- Updating of framework source code to use C#8 capabilities
- Removal of dependency on System.Dynamic, due to concerns that that might be cause of (very rare) runtime errors.
- Removal of redundant code
- Restructuring of packages

Under the new package structure, we will be publishing just four NuGet packages:

- NakedObjects.Server (will be released as version 11.0.0)
- NakedObjects.ProgrammingModel (will be released as 8.0.0.)	 
	Note however that this is identical to 7.0.4 - the current release - except compiled against .NET Core 3,
	so no changes to domain object model code are required. (Members obsoleted since 7.0 have now been removed).
- NakedObjects.Client (will be released as version 9.0.6, but this is a new package name, not any new code)
- NakedObjects.Snippets (replaces NakedObjects.IDE - contains only code snippets. Will be released as 1.0.0)

The Client and Server communicate via  RESTful API, which remains at version 1.1.

Building the Server
===================

Pending release of the packages, the server framework may be built from the command line using these four steps:

dotnet build ProgrammingModel.sln
dotnet pack "Programming Model\NakedObjects.ProgrammingModel.Package\NakedObjects.ProgrammingModel.Package.csproj"  --include-symbols --include-source

dotnet build Server.sln
dotnet pack Server\NakedObjects.Server.Package\NakedObjects.Server.Package.csproj --include-symbols --include-source

The built ProgrammingModel and Server packages may be found in the bin directories of their respective projects


