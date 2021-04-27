// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using NakedFramework.Architecture.Component;

namespace NakedFramework.Architecture.Framework {
    /// <summary>
    ///     Defines a service that provides easy access to the principal components of the framework.
    ///     An implementation of this service interface will be injected into any domain
    ///     object that needs it.
    /// </summary>
    public interface INakedObjectsFramework {
        IMessageBroker MessageBroker { get; }
        ISession Session { get; }
        ILifecycleManager LifecycleManager { get; }
        INakedObjectManager NakedObjectManager { get; }
        IServicesManager ServicesManager { get; }
        IObjectPersistor Persistor { get; }
        IEnumerable<IReflector> Reflectors { get; }
        IMetamodelManager MetamodelManager { get; }
        IDomainObjectInjector DomainObjectInjector { get; }
        ITransactionManager TransactionManager { get; }
        IFrameworkResolver FrameworkResolver { get; }
        IServiceProvider ServiceProvider { get; }
        string[] ServerTypes { get; }
        ReflectorType ReflectorType { get; }
    }
}