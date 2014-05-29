// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;

namespace NakedObjects.Xat {
    public interface ITestObject : ITestNaked, ITestHasActions, ITestHasProperties {
        string IconName { get; }

        ITestObject AssertIsImmutable();

        ITestObject AssertIsDescribedAs(string expectedDescription);

        ITestObject AssertIsType(Type expectedType);

        ITestObject AssertTitleEquals(string expectedTitle);

        object GetDomainObject();
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}