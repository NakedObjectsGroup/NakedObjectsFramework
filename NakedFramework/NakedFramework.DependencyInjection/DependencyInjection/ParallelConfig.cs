// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Core.Component;
using NakedFramework.ModelBuilding.Component;
using NakedFunctions;
using NakedObjects.Architecture.Component;
using NakedObjects.Core.Authentication;
using NakedObjects.Core.Component;
using NakedObjects.Core.Framework;
using NakedObjects.Core.Spec;
using NakedObjects.DependencyInjection.FacetFactory;
using NakedObjects.Meta.Component;
using NakedObjects.Meta.Menu;
using NakedObjects.Service;
using NakedObjects.Menu;

namespace NakedObjects.DependencyInjection.DependencyInjection {
    public static class ParallelConfig {
        public static void RegisterCoreSingletonTypes(IServiceCollection services) {
            services.AddSingleton<ISpecificationCache, ImmutableInMemorySpecCache>();
            services.AddSingleton<IMetamodel, Metamodel>();
            services.AddSingleton<IMetamodelBuilder, Metamodel>();
            services.AddSingleton<IMenuFactory, MenuFactory>();
            services.AddSingleton<IModelIntegrator, ModelIntegrator>();
            services.AddSingleton<IModelBuilder, ModelBuilder>();
            services.AddSingleton<FacetFactoryTypesProvider, FacetFactoryTypesProvider>();
            services.AddSingleton(typeof(IFacetFactoryOrder<>), typeof(FacetFactoryOrder<>));
            services.AddSingleton<IAllServiceList, AllServiceList>();
        }

        public static void RegisterCoreScopedTypes(IServiceCollection services) {
            services.AddScoped<INakedObjectAdapterMap, NakedObjectAdapterHashMap>();
            services.AddScoped<IIdentityAdapterMap, IdentityAdapterHashMap>();
            services.AddScoped<IDomainObjectInjector, DomainObjectContainerInjector>();
            services.AddScoped<SpecFactory, SpecFactory>();
            services.AddScoped<IMetamodelManager, MetamodelManager>();
            services.AddScoped<IPersistAlgorithm, FlatPersistAlgorithm>();
            services.AddScoped<IIdentityMap, IdentityMapImpl>();
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<INakedObjectManager, NakedObjectManager>();
            services.AddScoped<IObjectPersistor, ObjectPersistor>();
            services.AddScoped<IServicesManager, ServicesManager>();
            services.AddScoped<ILifecycleManager, LifeCycleManager>();

            services.AddScoped<IMessageBroker, MessageBroker>();
            services.AddScoped<INakedObjectsFramework, NakedObjectsFramework>();
            services.AddScoped<ISession, WindowsSession>();
            services.AddScoped<IFrameworkResolver>(p => new FrameworkResolver(p));

            //Temporary scaffolding
            services.AddScoped<NakedObjectFactory, NakedObjectFactory>();
            services.AddScoped<SpecFactory, SpecFactory>();
        }

        public static void RegisterFacetFactories<T>(this IServiceCollection services, Type[] facetFactories) where T : IFacetFactory {
            foreach (var factory in facetFactories) {
                services.RegisterFacetFactory<T>(factory);
            }
        }

        public static void RegisterWellKnownServices(IServiceCollection services) {
            services.AddScoped<IAlert, Alert>();
        }
    }
}