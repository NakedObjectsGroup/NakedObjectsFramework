// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Xat {
    public class TestObjectFactory : ITestObjectFactory {
        private readonly ILifecycleManager lifecycleManager;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodelManager;
        private readonly IObjectPersistor persistor;
        private readonly ITransactionManager transactionManager;

        public TestObjectFactory(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectManager manager, ITransactionManager transactionManager) {
            this.metamodelManager = metamodelManager;
            this.Session = session;
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.manager = manager;
            this.transactionManager = transactionManager;
        }

        #region ITestObjectFactory Members

        public ISession Session { get; set; }

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