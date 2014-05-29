// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Core.Context;

namespace NakedObjects.Xat {
    internal class TestService : TestHasActions, ITestService {
        public TestService(object service, ITestObjectFactory factory) : base(factory) {
            NakedObject = NakedObjectsContext.ObjectPersistor.GetAdapterFor(service);
        }

        #region ITestService Members

        public override string Title {
            get { return NakedObject.TitleString(); }
        }

        #endregion
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}