// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NakedFramework.Architecture.Component;
using NakedObjects.Core.Component;
using NakedObjects.Core.Spec;

namespace NakedObjects.Service {
    public sealed class NakedObjectsFramework : INakedObjectsFramework {
        public NakedObjectsFramework(IMessageBroker messageBroker,
                                     ISession session,
                                     ILifecycleManager lifecycleManager,
                                     IServicesManager servicesManager,
                                     INakedObjectManager nakedObjectManager,
                                     IObjectPersistor persistor,
                                     IEnumerable<IReflector> reflectors,
                                     IMetamodelManager metamodelManagerManager,
                                     IDomainObjectInjector domainObjectInjector,
                                     NakedObjectFactory nakedObjectFactory,
                                     SpecFactory memberFactory,
                                     ITransactionManager transactionManager,
                                     IFrameworkResolver frameworkResolver,
                                     ILoggerFactory loggerFactory,
                                     IServiceProvider serviceProvider) {
            MessageBroker = messageBroker;
            Session = session;
            LifecycleManager = lifecycleManager;
            ServicesManager = servicesManager;
            NakedObjectManager = nakedObjectManager;
            Persistor = persistor;
            Reflectors = reflectors;
            MetamodelManager = metamodelManagerManager;
            DomainObjectInjector = domainObjectInjector;
            TransactionManager = transactionManager;
            FrameworkResolver = frameworkResolver;
            ServiceProvider = serviceProvider;
            domainObjectInjector.Framework = this;
            memberFactory.Initialize(this, loggerFactory, loggerFactory.CreateLogger<SpecFactory>());
            nakedObjectFactory.Initialize(this, loggerFactory);
        }

        #region INakedObjectsFramework Members

        public IDomainObjectInjector DomainObjectInjector { get; }

        public ITransactionManager TransactionManager { get; }

        public IFrameworkResolver FrameworkResolver { get; }

        public IServiceProvider ServiceProvider { get; }

        public IMessageBroker MessageBroker { get; }

        public ISession Session { get; }

        public ILifecycleManager LifecycleManager { get; }

        public INakedObjectManager NakedObjectManager { get; }

        public IServicesManager ServicesManager { get; }

        public IObjectPersistor Persistor { get; }

        public IEnumerable<IReflector> Reflectors { get; }

        public IMetamodelManager MetamodelManager { get; }

        public string[] ServerTypes => Reflectors.Select(r => r.Name).ToArray();

        #endregion
    }
}