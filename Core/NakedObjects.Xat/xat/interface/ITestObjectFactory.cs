// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;

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

        ITestAction CreateTestAction(IActionSpec actionSpec, ITestHasActions owningObject);

        ITestParameter CreateTestParameter(IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject);

        ITestAction CreateTestAction(string contributor, IActionSpec actionSpec, ITestHasActions owningObject);

        ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject);
        ISession Session { get; set; }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}