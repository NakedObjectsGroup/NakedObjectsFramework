// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Persist;
using NakedObjects.Core.Util;

namespace NakedObjects.Xat {
    internal class TestParameterObject : ITestValue {
        private readonly INakedObjectManager manager;
        private readonly object domainObject;

        public TestParameterObject(INakedObjectManager manager,  object domainObject) {
            this.manager = manager;
            this.domainObject = domainObject;
        }

        #region ITestValue Members

        public string Title {
            get { return NakedObject.TitleString(); }
        }

        public INakedObject NakedObject {
            get { return manager.CreateAdapter(domainObject, null, null); }
            set { throw new UnexpectedCallException(); }
        }

        #endregion
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}