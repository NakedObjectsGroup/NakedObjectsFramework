// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Security.Principal;
using NakedObjects.Architecture.Security;

namespace NakedObjects.TestSystem {
    public class TestProxySession : ISession {
        #region ISession Members

        public string UserName {
            get { return "unit tester"; }
        }

        public bool IsAuthenticated {
            get { return true; }
        }

        public IPrincipal Principal {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}