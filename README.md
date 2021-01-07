Naked Objects Framework
=======================

Full documentation of how use the framework (typically starting from the Template projects) is contained in the Developer Manual (within the Documentation folder).
There is no need to download and build the source, as the recommended way to use the framework is via the published NuGet and NPM packages. (However there are details in the Developer Manual on how to build the source for those that want to.)

The latest release of the Naked Objects framework is v11.0.0.

The NuGet package structure has been simplified to just two packages:

- NakedObjects.Server (currently v11.0.0)
- NakedObjects.ProgrammingModel (currently v11.0.0.)	 
	Note however that this is identical to 7.0.4 - the previous release - except compiled against .NET Core 3,
	so no changes to domain object model code are required. (Members obsoleted since 7.0 have now been removed).
	
The Naked Objects Client is built from a set of NPM packages. 

The Client and Server communicate via  RESTful API, which remains at version 1.1.

In v11:
- All assemblies will are now built on .NET Core 3
- Dependency on System.Dynamic, has been removed, due to concerns that it might be cause of (very rare) runtime errors.
