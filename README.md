Naked Objects Framework
======================

The current stable release of the Naked Objects Framework (NOF) is version 8.3.1  -  see below for details.

The NOF may be used entirely as packages from the NuGet public gallery - there is no need to clone this repository.  Indeed, building the framework from source code is quite complex and not recommended for newcomers.  (If you really want to know how to do it, see the section 'How to build the framework from source code' in the Developer Manual.)

NOF9 (under development - master branch)
====

NOF9 will upgrade the Spa client to use Angular 2 instead of Angular 1.  The code for this new client is being developed in the 'Spa2' project within the master branch, so it does not interfere with the existing (NOF8) Spa project.  Ouor intention is that the Spa2 client will be identical to the NOF8 Spa from a user perspective, and there will be no change to the programming model  -  so the two clients can co-exist, talking to the same NOF8 server via the Restful Objects API. However, any custom views will need to be re-written to follow the Angular 2 patterns.


NOF8 (stable release - master branch)
====

NOF8  introduces a radically different user interface based on the Single Page Application (SPA) architecture. It uses identical domain model programming conventions as NOF7; indeed it is possible to run the NOF8 client and the NOF7 client alongside each other, as two different 'run' projects talking to the same domain model project(s).

The best way to try NOF8 is to run the NakedObjects.Template application, which may be downloaded as a .zip file from: https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/master/Run/NakedObjects.Template.zip?raw=true 

(If the unzipped application does not run first time, please see the developer manual for further hints. In particular, look for any server start-up errors in the log file: nakedobjects_log.txt. And also check the connection string in web.config).

The developer manual for NOF 8 is available here: .
https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/master/Documentation/NOF%208%20-%20DeveloperManual.docx?raw=true

When searching the NuGet package gallery for NOF8 please ensure you have the 'include pre-releases' checkbox selected.

NOF8 source code is held in the 8.0 branch of this repository.

NOF7 (superseded - 7.0 branch)
====

NOF7 has now been superseded and we advise all users to upgrade to NOF8.

NOF7 source code is available on the 7.0 branch of this repository.

The developer manual for NOF7 may be downloaded from here:
https://github.com/NakedObjectsGroup/NakedObjectsFramework/blob/7.0/Documentation/DeveloperManual.docx?raw=true


