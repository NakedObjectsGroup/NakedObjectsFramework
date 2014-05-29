// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;

namespace NakedObjects.Architecture.Security {
    public class NullAuthorizationManager : IAuthorizationManager {
        #region IAuthorizationManager Members

        public void Init() {
            // do nothing
        }

        public void Shutdown() {
            // do nothing
        }


        public bool IsVisible(ISession session, INakedObject target, IIdentifier identifier) {
            return true;
        }

        public bool IsEditable(ISession session, INakedObject target, IIdentifier identifier) {
            return true;
        }

        public void UpdateAuthorizationCache(INakedObject nakedObject) {
            // do nothing
        }

        #endregion
    }
}