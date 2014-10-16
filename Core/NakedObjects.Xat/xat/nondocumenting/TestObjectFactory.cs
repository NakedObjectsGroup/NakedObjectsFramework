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
        private readonly INakedObjectManager manager;
        private readonly INakedObjectTransactionManager transactionManager;

        public TestObjectFactory(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectManager manager, INakedObjectTransactionManager transactionManager) {
            this.metamodelManager = metamodelManager;
            this.Session = session;
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.manager = manager;
            this.transactionManager = transactionManager;
        }

        public ISession Session { get; set; }

        #region ITestObjectFactory Members

        public ITestService CreateTestService(Object service) {
            var no = manager.GetAdapterFor(service);
            return new TestService(no, lifecycleManager, this);
        }

        public ITestCollection CreateTestCollection(INakedObject instances) {
            return new TestCollection(instances, this, manager);
        }

        public ITestObject CreateTestObject(INakedObject nakedObject) {
            return new TestObject(lifecycleManager, persistor, nakedObject, this, transactionManager);
        }

        public ITestNaked CreateTestNaked(INakedObject nakedObject) {
            if (nakedObject == null) {
                return null;
            }
            if (nakedObject.Spec.IsParseable) {
                return CreateTestValue(nakedObject);
            }
            if (nakedObject.Spec.IsObject) {
                return CreateTestObject(nakedObject);
            }
            if (nakedObject.Spec.IsCollection) {
                return CreateTestCollection(nakedObject);
            }

            return null;
        }

        public ITestAction CreateTestAction(IActionSpec actionSpec, ITestHasActions owningObject) {
            return new TestAction(metamodelManager, Session, lifecycleManager, actionSpec, owningObject, this, manager, transactionManager);
        }

        public ITestAction CreateTestAction(string contributor, IActionSpec actionSpec, ITestHasActions owningObject) {
            return new TestAction(metamodelManager, Session, lifecycleManager, contributor, actionSpec, owningObject, this, manager, transactionManager);
        }

        public ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject) {
            return new TestProperty(lifecycleManager, Session, persistor, field, owningObject, this, manager);
        }

        public ITestParameter CreateTestParameter(IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject) {
            return new TestParameter(lifecycleManager, actionSpec, parameterSpec, owningObject, this);
        }

        #endregion

        private static ITestValue CreateTestValue(INakedObject nakedObject) {
            return new TestValue(nakedObject);
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}