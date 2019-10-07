// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Core.Component;
using NakedObjects.Core.Spec;

namespace NakedObjects.Service {
    public sealed class NakedObjectsFramework : INakedObjectsFramework {
        private readonly IDomainObjectInjector domainObjectInjector;
        private readonly IFrameworkResolver frameworkResolver;
        private readonly ILifecycleManager lifecycleManager;
        private readonly IMessageBroker messageBroker;
        private readonly IMetamodelManager metamodelManagerManager;
        private readonly INakedObjectManager nakedObjectManager;
        private readonly IObjectPersistor persistor;
        private readonly IReflector reflector;
        private readonly IServicesManager servicesManager;
        private readonly ISession session;
        private readonly ITransactionManager transactionManager;

        public NakedObjectsFramework(IMessageBroker messageBroker,
                                     ISession session,
                                     ILifecycleManager lifecycleManager,
                                     IServicesManager servicesManager,
                                     INakedObjectManager nakedObjectManager,
                                     IObjectPersistor persistor,
                                     IReflector reflector,
                                     IMetamodelManager metamodelManagerManager,
                                     IDomainObjectInjector domainObjectInjector,
                                     NakedObjectFactory nakedObjectFactory,
                                     SpecFactory memberFactory,
                                     ITransactionManager transactionManager,
                                     IFrameworkResolver frameworkResolver) {
            this.messageBroker = messageBroker;
            this.session = session;
            this.lifecycleManager = lifecycleManager;
            this.servicesManager = servicesManager;
            this.nakedObjectManager = nakedObjectManager;
            this.persistor = persistor;
            this.reflector = reflector;
            this.metamodelManagerManager = metamodelManagerManager;
            this.domainObjectInjector = domainObjectInjector;
            this.transactionManager = transactionManager;
            this.frameworkResolver = frameworkResolver;
            domainObjectInjector.Framework = this;
            memberFactory.Initialize(this);
            nakedObjectFactory.Initialize(metamodelManagerManager, session, lifecycleManager, persistor, nakedObjectManager);
        }

        #region INakedObjectsFramework Members

        public IDomainObjectInjector DomainObjectInjector {
            get { return domainObjectInjector; }
        }

        public ITransactionManager TransactionManager {
            get { return transactionManager; }
        }

        public IFrameworkResolver FrameworkResolver {
            get { return frameworkResolver; }
        }

        public IMessageBroker MessageBroker {
            get { return messageBroker; }
        }

        public ISession Session {
            get { return session; }
        }

        public ILifecycleManager LifecycleManager {
            get { return lifecycleManager; }
        }

        public INakedObjectManager NakedObjectManager {
            get { return nakedObjectManager; }
        }

        public IServicesManager ServicesManager {
            get { return servicesManager; }
        }

        public IObjectPersistor Persistor {
            get { return persistor; }
        }

        public IReflector Reflector {
            get { return reflector; }
        }

        public IMetamodelManager MetamodelManager {
            get { return metamodelManagerManager; }
        }

        #endregion
    }
}