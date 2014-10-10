// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Persist;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Security;
using NakedObjects.Core.Context;
using NakedObjects.Core.Reflect;
using NakedObjects.Core.spec;
using NakedObjects.Managers;
using NakedObjects.Objects;

namespace NakedObjects.Service {
    public class NakedObjectsFramework : INakedObjectsFramework {
        private readonly IAuthorizationManager authorizationManager;
        private readonly IMetamodelManager metamodelManager;
        private readonly IContainerInjector injector;
        private readonly IMessageBroker messageBroker;
        private readonly ILifecycleManager lifecycleManager;
        private readonly IServicesManager services;
        private readonly INakedObjectManager manager;
        private readonly IObjectPersistor persistor;
        private readonly INakedObjectReflector reflector;
        private readonly ISession session;
        private readonly IUpdateNotifier updateNotifier;

        public NakedObjectsFramework(IMessageBroker messageBroker,
                                     IUpdateNotifier updateNotifier,
                                     ISession session,
                                     ILifecycleManager lifecycleManager,
                                     IServicesManager services,
                                     INakedObjectManager manager,
                                     IObjectPersistor persistor,
                                     INakedObjectReflector reflector,
                                     IAuthorizationManager authorizationManager,
                                     IMetamodelManager metamodelManager,
                                     IContainerInjector injector,
                                     NakedObjectFactory  nakedObjectFactory,
                                     MemberFactory memberFactory) {
            this.messageBroker = messageBroker;
            this.updateNotifier = updateNotifier;
            this.session = session;
            this.lifecycleManager = lifecycleManager;
            this.services = services;
            this.manager = manager;
            this.persistor = persistor;
            this.reflector = reflector;
            this.authorizationManager = authorizationManager;
            this.metamodelManager = metamodelManager;
            this.injector = injector;
            injector.Framework = this;
            memberFactory.Initialize(this);
            nakedObjectFactory.Initialize(metamodelManager, session, lifecycleManager, persistor);
        }

        #region INakedObjectsFramework Members

        public IContainerInjector Injector {
            get { return injector; }
        }

        public IMessageBroker MessageBroker {
            get { return messageBroker; }
        }

        public IUpdateNotifier UpdateNotifier {
            get { return updateNotifier; }
        }

        public ISession Session {
            get { return session; }
        }

        public ILifecycleManager LifecycleManager {
            get { return lifecycleManager; }
        }

        public INakedObjectManager Manager {
            get { return manager; }
        }

        public IServicesManager Services {
            get { return services; }
        }

        public IObjectPersistor Persistor {
            get { return persistor; }
        }

        public INakedObjectReflector Reflector {
            get { return reflector; }
        }

        public IMetamodelManager Metamodel {
            get { return metamodelManager; }
        }

        public IAuthorizationManager AuthorizationManager {
            get { return authorizationManager; }
        }

        #endregion
    }
}