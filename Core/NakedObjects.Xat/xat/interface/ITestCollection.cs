// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Collections.Generic;
using System.Linq;

namespace NakedObjects.Xat {
    public interface ITestCollection : ITestNaked, IEnumerable<ITestObject> {
        ITestCollection AssertIsEmpty();

        ITestCollection AssertIsNotEmpty();

        ITestCollection AssertCountIs(int count);
        ITestCollection AssertIsTransient();
        ITestCollection AssertIsPersistent();
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}