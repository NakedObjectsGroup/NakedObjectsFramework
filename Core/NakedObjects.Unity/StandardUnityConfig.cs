// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Authentication;
using NakedObjects.Core.Component;
using NakedObjects.Core.Spec;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.Menu;
using NakedObjects.Persistor.Entity.Component;
using NakedObjects.Reflect;
using NakedObjects.Reflect.Component;
using NakedObjects.Service;
using Microsoft.Practices.Unity;


namespace NakedObjects.Unity {
    public static class StandardUnityConfig {
        public static void RegisterCoreContainerControlledTypes(IUnityContainer container) {
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMenuFactory, MenuFactory>(new ContainerControlledLifetimeManager());
        }

        public static void RegisterCorePerTransactionTypes<T>(IUnityContainer container)
            where T : LifetimeManager, new() {
            container.RegisterType<INakedObjectAdapterMap, NakedObjectAdapterHashMap>(new T(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new T(), new InjectionConstructor(10));
            container.RegisterType<IDomainObjectInjector, DomainObjectContainerInjector>(new T());
            container.RegisterType<IOidGenerator, EntityOidGenerator>(new T());
            container.RegisterType<IPersistAlgorithm, FlatPersistAlgorithm>(new T());
            container.RegisterType<IObjectStore, EntityObjectStore>(new T());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new T());
            container.RegisterType<ITransactionManager, TransactionManager>(new T());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new T());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new T());
            container.RegisterType<IServicesManager, ServicesManager>(new T());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new T());
            container.RegisterType<IMetamodelManager, MetamodelManager>(new T());
            container.RegisterType<IMessageBroker, MessageBroker>(new T());
            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new T());
            container.RegisterType<ISession, WindowsSession>(new T());
            container.RegisterType<IFrameworkResolver, UnityFrameworkResolver>(new T());

            //Temporary scaffolding
            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new T());
            container.RegisterType<SpecFactory, SpecFactory>(new T());
        }

        public static void RegisterStandardFacetFactories(IUnityContainer container) {
            var factoryTypes = FacetFactories.StandardFacetFactories();
            for (int i = 0; i < factoryTypes.Count(); i++) {
                UnityConfigHelpers.RegisterFacetFactory(factoryTypes[i], container, i);
            }
        }
    }
}