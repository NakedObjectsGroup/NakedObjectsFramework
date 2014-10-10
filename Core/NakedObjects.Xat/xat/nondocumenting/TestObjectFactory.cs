// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;

namespace NakedObjects.Xat {
    public class TestObjectFactory : ITestObjectFactory {
        private readonly IMetamodelManager metamodelManager;
      
        private readonly ILifecycleManager lifecycleManager;
        private readonly IObjectPersistor persistor;

        public TestObjectFactory(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor) {
            this.metamodelManager = metamodelManager;
            this.Session = session;
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
        }

        public ISession Session { get; set; }

        #region ITestObjectFactory Members

        public ITestService CreateTestService(Object service) {
            var no = lifecycleManager.GetAdapterFor(service);
            return new TestService(no, lifecycleManager, this);
        }

        public ITestCollection CreateTestCollection(INakedObject instances) {
            return new TestCollection(instances, this, lifecycleManager);
        }

        public ITestObject CreateTestObject(INakedObject nakedObject) {
            return new TestObject(lifecycleManager, persistor, nakedObject, this);
        }

        public ITestNaked CreateTestNaked(INakedObject nakedObject) {
            if (nakedObject == null) {
                return null;
            }
            if (nakedObject.Specification.IsParseable) {
                return CreateTestValue(nakedObject);
            }
            if (nakedObject.Specification.IsObject) {
                return CreateTestObject(nakedObject);
            }
            if (nakedObject.Specification.IsCollection) {
                return CreateTestCollection(nakedObject);
            }

            return null;
        }

        public ITestAction CreateTestAction(INakedObjectAction action, ITestHasActions owningObject) {
            return new TestAction(metamodelManager, Session, lifecycleManager, action, owningObject, this);
        }

        public ITestAction CreateTestAction(string contributor, INakedObjectAction action, ITestHasActions owningObject) {
            return new TestAction(metamodelManager, Session, lifecycleManager, contributor, action, owningObject, this);
        }

        public ITestProperty CreateTestProperty(INakedObjectAssociation field, ITestHasActions owningObject) {
            return new TestProperty(lifecycleManager, Session, persistor, field, owningObject, this);
        }

        public ITestParameter CreateTestParameter(INakedObjectAction action, INakedObjectActionParameter parameter, ITestHasActions owningObject) {
            return new TestParameter(lifecycleManager, action, parameter, owningObject, this);
        }

        #endregion

        private static ITestValue CreateTestValue(INakedObject nakedObject) {
            return new TestValue(nakedObject);
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}