using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Testlib;
using org.nakedobjects.xat.documentor;

namespace NakedObjects.Xat.Generic {
    /// <summary>
    /// Creates test objects for XAT, also is expected to cache the <see cref="IDocumentor"/> statically so is available
    /// by <see cref="AcceptanceTestCase"/>
    /// <para>
    /// This interface is not used by XAT2.
    /// </para>
    public interface ITestObjectFactory : IDocumentorFactory {
     
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