// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Xat {
    /// <summary>
    /// Creates test objects for XAT
    /// This interface is not used by XAT2.
    /// </summary>
    public interface ITestObjectFactory {
        ITestService CreateTestService(Object service);

        ITestCollection CreateTestCollection(INakedObject instances);

        ITestObject CreateTestObject(INakedObject nakedObject);

        ITestNaked CreateTestNaked(INakedObject nakedObject);

        ITestAction CreateTestAction(INakedObjectAction action, ITestHasActions owningObject);

        ITestParameter CreateTestParameter(INakedObjectAction action, INakedObjectActionParameter parameter, ITestHasActions owningObject);

        ITestAction CreateTestAction(string contributor, INakedObjectAction action, ITestHasActions owningObject);

        ITestProperty CreateTestProperty(INakedObjectAssociation field, ITestHasActions owningObject);
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}