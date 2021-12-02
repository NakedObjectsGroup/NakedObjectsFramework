// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.Extensions.DependencyInjection;
using NakedFramework.Architecture.Component;
using NakedFramework.Architecture.Framework;
using NakedFramework.Core.Authentication;
using NakedFramework.Core.Component;
using NakedFramework.Core.Spec;
using NakedFramework.DependencyInjection.Component;
using NakedFramework.DependencyInjection.FacetFactory;
using NakedFramework.DependencyInjection.Utils;
using NakedFramework.Menu;
using NakedFramework.Metamodel.Component;
using NakedFramework.Metamodel.Menu;
using NakedFramework.ModelBuilding.Component;

namespace NakedFramework.DependencyInjection.Configuration;

public static class ParallelConfig {
    public static void RegisterCoreSingletonTypes(IServiceCollection services) {
        services.AddDefaultSingleton<ISpecificationCache, ImmutableInMemorySpecCache>();
        services.AddDefaultSingleton<IMetamodel, MetamodelHolder>();
        services.AddDefaultSingleton<IMetamodelBuilder, MetamodelHolder>();
        services.AddDefaultSingleton<IMenuFactory, MenuFactory>();
        services.AddDefaultSingleton<IModelIntegrator, ModelIntegrator>();
        services.AddDefaultSingleton<IModelBuilder, ModelBuilder>();
        services.AddDefaultSingleton<FacetFactoryTypesProvider, FacetFactoryTypesProvider>();
        services.AddDefaultSingleton(typeof(IFacetFactoryOrder<>), typeof(FacetFactoryOrder<>));
        services.AddDefaultSingleton<IAllServiceList, AllServiceList>();
    }

    public static void RegisterCoreScopedTypes(IServiceCollection services) {
        services.AddDefaultScoped<INakedObjectAdapterMap, NakedObjectAdapterHashMap>();
        services.AddDefaultScoped<IIdentityAdapterMap, IdentityAdapterHashMap>();
        services.AddDefaultScoped<SpecFactory, SpecFactory>();
        services.AddDefaultScoped<IMetamodelManager, MetamodelManager>();
        services.AddDefaultScoped<IPersistAlgorithm, FlatPersistAlgorithm>();
        services.AddDefaultScoped<IIdentityMap, IdentityMapImpl>();
        services.AddDefaultScoped<ITransactionManager, TransactionManager>();
        services.AddDefaultScoped<INakedObjectManager, NakedObjectManager>();
        services.AddDefaultScoped<IObjectPersistor, ObjectPersistor>();
        services.AddDefaultScoped<IServicesManager, ServicesManager>();
        services.AddDefaultScoped<ILifecycleManager, LifeCycleManager>();

        services.AddDefaultScoped<IMessageBroker, MessageBroker>();
        services.AddDefaultScoped<INakedFramework, Core.Framework.NakedFramework>();
        services.AddDefaultScoped<ISession, WindowsSession>();
        services.AddScoped<IFrameworkResolver>(p => new FrameworkResolver(p));

        //Temporary scaffolding
        services.AddDefaultScoped<NakedObjectFactory, NakedObjectFactory>();
        services.AddDefaultScoped<SpecFactory, SpecFactory>();
    }

    public static void RegisterFacetFactories<T>(this IServiceCollection services, Type[] facetFactories) where T : IFacetFactory {
        foreach (var factory in facetFactories) {
            services.RegisterFacetFactory<T>(factory);
        }
    }
}