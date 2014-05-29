// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Xat {
    internal class TestValue : ITestValue {
        public TestValue(INakedObject nakedObject) {
            NakedObject = nakedObject;
        }

        #region ITestValue Members

        public string Title {
            get { return NakedObject.TitleString(); }
        }

        public INakedObject NakedObject { get; private set; }

        #endregion
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}