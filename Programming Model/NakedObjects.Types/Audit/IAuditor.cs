// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Security.Principal;

namespace NakedObjects.Audit {
    /// <summary>
    /// Allows domain programmers to define an auditing service (registered via the
    /// AuditorInstaller on the Run class). 
    /// </summary>
    public interface IAuditor {
        void ActionInvoked(IPrincipal byPrincipal, string actionName, object onObject, bool queryOnly, object[] withParameters);
        void ActionInvoked(IPrincipal byPrincipal, string actionName, string serviceName, bool queryOnly, object[] withParameters);
        void ObjectUpdated(IPrincipal byPrincipal, object updatedObject);
        void ObjectPersisted(IPrincipal byPrincipal, object updatedObject);
    }
}