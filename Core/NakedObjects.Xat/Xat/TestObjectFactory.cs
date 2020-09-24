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
        private readonly IMessageBroker messageBroker;
        private readonly IMetamodelManager metamodelManager;
        private readonly IObjectPersistor persistor;
        private readonly IServicesManager servicesManager;
        private readonly ITransactionManager transactionManager;

        public TestObjectFactory(IMetamodelManager metamodelManager, ISession session, ILifecycleManager lifecycleManager, IObjectPersistor persistor, INakedObjectManager manager, ITransactionManager transactionManager, IServicesManager servicesManager, IMessageBroker messageBroker) {
            this.metamodelManager = metamodelManager;
            Session = session;
            this.lifecycleManager = lifecycleManager;
            this.persistor = persistor;
            this.manager = manager;
            this.transactionManager = transactionManager;
            this.servicesManager = servicesManager;
            this.messageBroker = messageBroker;
        }

        #region ITestObjectFactory Members

        public ISession Session { get; set; }

        public ITestService CreateTestService(object service) {
            var no = manager.GetServiceAdapter(service);
            Assert.IsNotNull(no);
            return CreateTestService(no);
        }

        public ITestMenu CreateTestMenuMain(IMenuImmutable menu) => new TestMenu(menu, this, null);

        public ITestMenu CreateTestMenuForObject(IMenuImmutable menu, ITestHasActions owningObject) => new TestMenu(menu, this, owningObject);

        public ITestMenuItem CreateTestMenuItem(IMenuItemImmutable item, ITestHasActions owningObject) => new TestMenuItem(item, this, owningObject);

        public ITestCollection CreateTestCollection(INakedObjectAdapter instances) => new TestCollection(instances, this, manager);

        public ITestObject CreateTestObject(INakedObjectAdapter nakedObjectAdapter) => new TestObject(lifecycleManager, persistor, nakedObjectAdapter, this, transactionManager);

        public ITestNaked CreateTestNaked(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter == null) {
                return null;
            }

            if (nakedObjectAdapter.Spec.IsParseable) {
                return CreateTestValue(nakedObjectAdapter);
            }

            if (nakedObjectAdapter.Spec.IsObject) {
                return CreateTestObject(nakedObjectAdapter);
            }

            if (nakedObjectAdapter.Spec.IsCollection) {
                return CreateTestCollection(nakedObjectAdapter);
            }

            return null;
        }

        public ITestAction CreateTestAction(IActionSpec actionSpec, ITestHasActions owningObject) => new TestAction(metamodelManager, Session, lifecycleManager, transactionManager, actionSpec, owningObject, this, manager, messageBroker, servicesManager);

        public ITestAction CreateTestAction(IActionSpecImmutable actionSpecImm, ITestHasActions owningObject) {
            var actionSpec = metamodelManager.GetActionSpec(actionSpecImm);
            return CreateTestAction(actionSpec, owningObject);
        }

        public ITestAction CreateTestActionOnService(IActionSpecImmutable actionSpecImm) {
            var objectIm = actionSpecImm.OwnerSpec; //This is the spec for the service

            if (!(objectIm is IServiceSpecImmutable)) {
                throw new Exception("Action is not on a known service");
            }

            var serviceSpec = (IServiceSpec) metamodelManager.GetSpecification(objectIm);
            var service = servicesManager.GetService(serviceSpec);
            var testService = CreateTestService(service);
            return CreateTestAction(actionSpecImm, testService);
        }

        public ITestAction CreateTestAction(string contributor, IActionSpec actionSpec, ITestHasActions owningObject) => new TestAction(metamodelManager, Session, lifecycleManager, transactionManager, contributor, actionSpec, owningObject, this, manager, messageBroker, servicesManager);

        public ITestProperty CreateTestProperty(IAssociationSpec field, ITestHasActions owningObject) => new TestProperty(persistor, field, owningObject, this, manager);

        public ITestParameter CreateTestParameter(IActionSpec actionSpec, IActionParameterSpec parameterSpec, ITestHasActions owningObject) => new TestParameter(parameterSpec, owningObject, this);

        #endregion

        public ITestService CreateTestService(INakedObjectAdapter service) => new TestService(service, this);

        private static ITestValue CreateTestValue(INakedObjectAdapter nakedObjectAdapter) => new TestValue(nakedObjectAdapter);
    }

    // Copyright (c) Naked Objects Group Ltd.
}