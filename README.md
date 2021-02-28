This repository is home to two C# application development frameworks:  Naked Objects, and Naked Functions. With Naked Objects you write your application code in pure object-oriented style; with Naked Functions you write it in pure functional programming style.  But it is what they have in common that distinguishes them from other application development frameworks:

•	You write only domain types and logic. For Naked Objects that means classes representing persistent domain entities and view models, with all your domain logic encapsulated as methods. For Naked Functions it means writing C# records (or immutable classes) and freestanding (static) functions that are 100% side-effect free. 
•	Persistence is managed through Entity Framework - either EF 6 or (imminently) EF Core.
•	Using introspection (during start-up only) the frameworks generate a complete RESTful API for the domain code.
•	A generic client consumes this RESTful API to provide a rich user interface. The client, which is common to both Naked Objects and Naked Functions, is written in Angular and runs as a Single Page Application (SPA). 
•	The generic client may be customised for look and feel using standard Angular patterns, and the beauty of the design is that this customisation may be undertaken completely independently of the domain application development. Many users have found that there is no need to customise it at all: the generic client is good enough for deployment. At the other extreme, since the Client adopts a well-structured layered architecture (each layer being a separate NPM package) you may choose to build your SPA from scratch, using only the lower layers of the generic Client architecture as helpers to interact with the RESTful API.
•	The frameworks may be used ready-packaged (as NuGet packages for the server side, and as NPM packages for the client) – there is no need to download the source code from this repository. (See instructions below).

Both frameworks therefore offer the following advantages:

•	Rapid development cycle
•	Easier maintenance: changing the domain model does not require any changes to the UI code (or to the persistence layer if you adopt Entity Framework best practices).
•	100% consistent UI look and feel, across large, complex, domain models or multiple applications
•	Stateless server operation with all the deployment benefits of using a pure RESTful API
•	Improved communication between users and developers because the UI corresponds directly to the underlying domain model.



Naked Objects
=============
Naked Objects is a mature framework, under continuous development for 20 years (last 7 on GitHub) and now at version 11.

Full documentation of how use the framework (typically starting from the Template projects) is contained in the Developer Manual (within the Documentation folder).
There is no need to download and build the source, as the recommended way to use the framework is via the published NuGet and NPM packages. (However there are details in the Developer Manual on how to build the source for those that want to.)

As of v11, all assemblies will are now built on .NET Core 3. Dependency on System.Dynamic, has been removed, due to concerns that it might be cause of (very rare) runtime errors. The NuGet package structure has also been simplified to just two packages:

- NakedObjects.Server (currently v11.0.0)
- NakedObjects.ProgrammingModel (currently v11.0.0.)	 
	Note however that this is identical to 7.0.4 - the previous release - except compiled against .NET Core 3,
	so no changes to domain object model code are required. (Members obsoleted since 7.0 have now been removed).
	
Naked Functions
===============

Naked Functions is a brand new framework, currently at Beta stage.

While the full Application Developer Manual is currently under construction, the following PowerPoint slides illustrate the programming patterns used to construct an application using Naked Functions, and how the domain code relates to the UI:

An Introduction to Naked Functions (in the Documentation folder)



