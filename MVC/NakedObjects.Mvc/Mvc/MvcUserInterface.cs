// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Security;
using NakedObjects.Boot;

namespace NakedObjects.Web.Mvc {
    public class MvcUserInterface : INakedObjectsClient {
        #region INakedObjectsClient Members

        public void StartClient(NakedObjectsSystem system) {
            ISession session = system.AuthenticationManager.Authenticate();
            system.Connect(session);
        }

        #endregion
    }
}