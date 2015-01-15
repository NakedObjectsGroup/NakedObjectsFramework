// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Xat {
    public class TestObjectFactory : ITestObjectFactory {
        private readonly ILifecycleManager lifecycleManager;
        private readonly INakedObjectManager manager;
        private readonly IMetamodelManager metamodelManager;
        private readonly IObjectPersistor persistor;
        private readonly IServicesManager servicesManager;
        private readonly ITransactionManager transactionManager;

        public TestObjectFactory(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectManager manager, ITransactionManager transactionManager, IServicesManager servicesManager) {
            this.metamodelManager = metamodelManager;
            Session = session;
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.manager = manager;
            this.transactionManager = transactionManager;
            this.servicesManager = servicesManager;
        }

        #region ITestObjectFactory Members

        public ISession Session { get; set; }

        public ITestService CreateTestService(Object service) {
            INakedObject no = manager.GetServiceAdapter(service);
            Assert.IsNotNull(no);
            return CreateTestService(no);
        }

        public ITestMenu CreateTestMenuMain(IMenuImmutable menu) {
            return new TestMenu(menu, this, null);
        }

        public ITestMenu CreateTestMenuForObject(IMenuImmutable menu, ITestHasActions owningObject) {
            return new TestMenu(menu, this, owningObject);
        }

        public ITestMenuItem CreateTestMenuItem(IMenuItemImmutable item, ITestHasActions owningObject) {
            return new TestMenuItem(item, this, owningObject);
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
            return new TestAction(metamodelManager, Session, lifecycleManager, actionSpec, owningObject, this, manager);
        }

        public ITestAction CreateTestAction(IActionSpecImmutable actionSpec, ITestHasActions owningObject) {
            throw new NotImplementedException();
        }

        public ITestAction CreateTestActionOnService(IActionSpecImmutable actionSpecImm) {
            IObjectSpecImmutable objectIm = actionSpecImm.ReturnSpec; //This is the spec for the service

            if (!objectIm.Service) {
                throw new Exception("Action is not on a known object or service");
            }
            IObjectSpec objectSpec = metamodelManager.GetSpecification(objectIm);
            INakedObject service = servicesManager.GetService(objectSpec);
            ITestService testService = CreateTestService(service);
            IActionSpec actionSpec = metamodelManager.GetActionSpec(actionSpecImm);
            return CreateTestAction(actionSpec, testService);
        }

        public ITestAction CreateTestAction(string contributor, IActionSpec actionSpec, ITestHasActions owningObject) {
            return new TestAction(metamodelManager, Session, lifecycleManager, contributor, actionSpec, owningObject, this, manager);
        }

        public ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject) {
            return new TestProperty(persistor, field, owningObject, this, manager);
        }

        public ITestParameter CreateTestParameter(IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject) {
            return new TestParameter(parameterSpec, owningObject, this);
        }

        #endregion

        public ITestService CreateTestService(INakedObject service) {
            return new TestService(service, this);
        }

        private static ITestValue CreateTestValue(INakedObject nakedObject) {
            return new TestValue(nakedObject);
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}