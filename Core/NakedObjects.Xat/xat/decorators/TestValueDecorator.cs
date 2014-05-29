// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Xat {
    public class TestValueDecorator : ITestValue {
        private readonly ITestValue wrappedObject;

        protected TestValueDecorator(ITestValue wrappedObject) {
            this.wrappedObject = wrappedObject;
        }

        #region ITestValue Members

        public INakedObject NakedObject {
            get { return wrappedObject.NakedObject; }
        }

        public string Title {
            get { return wrappedObject.Title; }
        }

        #endregion
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}