This repository is now home to two .NET application development frameworks: **Naked Objects**, and **Naked Functions**. With Naked Objects you write your application code in pure object-oriented style; with Naked Functions you write it in pure functional programming style.  But it is what they have in common that distinguishes them from other application development frameworks:

* You write only domain types and logic. For Naked Objects that means classes representing persistent domain entities and view models, with all your domain logic encapsulated as methods. For Naked Functions it means writing C# records (or immutable classes) and freestanding (static) functions that are 100% side-effect free. 

* Persistence is managed through Entity Framework Core or Entity Framework 6

* Using introspection (during start-up) the frameworks generate a complete RESTful API for the domain code.

* A generic client consumes this RESTful API to provide a rich user interface. The client, which is common to both Naked Objects and Naked Functions, is written in Angular 12 and runs as a Single Page Application (SPA). 

* The generic client may be customised for look and feel using standard Angular patterns, and the beauty of the design is that this customisation may be undertaken completely independently of the domain application development. Many users have found that there is no need to customise it at all: the generic client is good enough for deployment. At the other extreme, since the Client adopts a well-structured layered architecture (each layer being a separate NPM package) you may choose to build your SPA from scratch, using only the lower layers of the generic Client architecture as helpers to interact with the RESTful API.

* The frameworks may be used ready-packaged (as NuGet packages for the server side, and as NPM packages for the client) – there is no need to download the source code from this repository.

Both frameworks therefore offer the following advantages:

* Rapid development cycle.

* Easier maintenance: changing the domain model does not require any changes to the UI code (or to the persistence layer if you adopt Entity Framework best practices).

* 100% consistent UI look and feel, across large, complex, domain models or multiple applications.

* Stateless server operation with all the deployment benefits of using a pure RESTful API.

* Improved communication between users and developers during development/maintenance because the UI corresponds directly to the underlying domain model.

Naked Objects
=============
Naked Objects is a mature framework, under continuous development for 20 years (last 7 on GitHub) and now at version 16.

Full documentation of how use the framework (typically starting from the Template projects) is contained in the [Naked Objects - Developer Manual](https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/master/Documentation/Naked%20Objects%20-%20Developer%20Manual.docx).
There is no need to download and build the source, as the recommended way to use the framework is via the published NuGet and NPM packages. (However there are details in the manual on how to build the source for those that really want to.)


Naked Functions
===============

Naked Functions is a brand new framework, currently at release 4.0. It depends upon .NET 8 and Entity Framework Core.

Full documentation of how use the framework (typically starting from the Template projects) is contained in the  [Naked Functions - Developer Manual](https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/master/Documentation/Naked%20Functions%20-%20Developer%20Manual.docx). There is no need to download and build the source, as the recommended way to use the framework is via the published NuGet and NPM packages.


