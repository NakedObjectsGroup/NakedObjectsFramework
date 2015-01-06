// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Practices.Unity;
using NakedObjects;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Authentication;
using NakedObjects.Core.Component;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Container;
using NakedObjects.Core.Spec;
using NakedObjects.Meta;
using NakedObjects.Meta.Audit;
using NakedObjects.Meta.Menu;
using NakedObjects.Persistor.Entity;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflect;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;
using NakedObjects.Service;
using NakedObjects.Surface;
using NakedObjects.Surface.Nof4.Implementation;
using NakedObjects.Surface.Nof4.Utility;
using NakedObjects.Meta.Authorization;

namespace NakedObjects.App.Demo {
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig {
        #region Framework Configuration

        protected static void RegisterFacetFactories(IUnityContainer container) {
            int order = 0;
            RegisterFacetFactory<FallbackFacetFactory>(container, order++);
            RegisterFacetFactory<IteratorFilteringFacetFactory>(container, order++);
            RegisterFacetFactory<UnsupportedParameterTypesMethodFilteringFactory>(container, order++);
            RegisterFacetFactory<RemoveSuperclassMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<RemoveInitMethodFacetFactory>(container, order++);
            RegisterFacetFactory<RemoveDynamicProxyMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<RemoveEventHandlerMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<TypeMarkerFacetFactory>(container, order++);
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            RegisterFacetFactory<MandatoryDefaultFacetFactory>(container, order++);
            RegisterFacetFactory<PropertyValidateDefaultFacetFactory>(container, order++);
            RegisterFacetFactory<ComplementaryMethodsFilteringFacetFactory>(container, order++);
            RegisterFacetFactory<ActionMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<CollectionFieldMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<PropertyMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<IconMethodFacetFactory>(container, order++);
            RegisterFacetFactory<CallbackMethodsFacetFactory>(container, order++);
            RegisterFacetFactory<TitleMethodFacetFactory>(container, order++);
            RegisterFacetFactory<ValidateObjectFacetFactory>(container, order++);
            RegisterFacetFactory<ComplexTypeAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<ViewModelFacetFactory>(container, order++);
            RegisterFacetFactory<BoundedAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<EnumFacetFactory>(container, order++);
            RegisterFacetFactory<ActionDefaultAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<PropertyDefaultAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<DescribedAsAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<DisabledAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<PasswordAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<ExecutedAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<PotencyAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<PageSizeAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<HiddenAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<HiddenDefaultMethodFacetFactory>(container, order++);
            RegisterFacetFactory<DisableDefaultMethodFacetFactory>(container, order++);
            RegisterFacetFactory<AuthorizeAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<ValidateProgrammaticUpdatesAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<ImmutableAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<MaxLengthAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<RangeAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<MemberOrderAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<MultiLineAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<NamedAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<NotPersistedAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<ProgramPersistableOnlyAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<OptionalAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<RequiredAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<PluralAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<DefaultNamingFacetFactory>(container, order++);// must come after Named and Plural factories
            RegisterFacetFactory<ConcurrencyCheckAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<ContributedActionAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<FinderActionFacetFactory>(container, order++);
            // must come after any facets that install titles
            RegisterFacetFactory<MaskAnnotationFacetFactory>(container, order++);
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            order = RegisterFacetFactory<FinderActionFacetFactory>(container, order++);
            RegisterFacetFactory<TypeOfAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<TableViewAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<TypicalLengthDerivedFromTypeFacetFactory>(container, order++);
            RegisterFacetFactory<TypicalLengthAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<EagerlyAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<PresentationHintAnnotationFacetFactory>(container, order++);
            RegisterFacetFactory<BooleanValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<ByteValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<SbyteValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<ShortValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<IntValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<LongValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<UShortValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<UIntValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<ULongValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<FloatValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<DoubleValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<DecimalValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<CharValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<DateTimeValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<TimeValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<StringValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<GuidValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<EnumValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<FileAttachmentValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<ImageValueTypeFacetFactory>(container, order++);
            RegisterFacetFactory<ArrayValueTypeFacetFactory<byte>>(container, order++);
            RegisterFacetFactory<CollectionFacetFactory>(container, order++);
            RegisterFacetFactory<MenuFacetFactory>(container, order++);
        }

        private static int RegisterFacetFactory<TFactory>(IUnityContainer container, int order) 
            where TFactory : IFacetFactory
        {
            container.RegisterType<IFacetFactory, TFactory>(typeof(TFactory).Name, new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            return order;
        }

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container) {
            RegisterFacetFactories(container);

            // config
            container.RegisterInstance<IReflectorConfiguration>(NakedObjectsRunSettings.ReflectorConfig(), (new ContainerControlledLifetimeManager()));
            container.RegisterInstance<IEntityObjectStoreConfiguration>(NakedObjectsRunSettings.EntityObjectStoreConfig(), new ContainerControlledLifetimeManager());
           
            // in architecture
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());
            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMenuFactory, MenuFactory>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerRequestLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new PerRequestLifetimeManager());
            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerRequestLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectStore, EntityObjectStore>(new PerRequestLifetimeManager());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new PerRequestLifetimeManager());
            container.RegisterType<ITransactionManager, TransactionManager>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new PerRequestLifetimeManager());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new PerRequestLifetimeManager());
            container.RegisterType<IServicesManager, ServicesManager>(new PerRequestLifetimeManager());
            container.RegisterType<ILifecycleManager, LifeCycleManager>(new PerRequestLifetimeManager());
            container.RegisterType<IMetamodelManager, MetamodelManager>(new PerResolveLifetimeManager());
            container.RegisterType<ISession, WindowsSession>(new PerRequestLifetimeManager());
            container.RegisterType<IMessageBroker, MessageBroker>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new PerRequestLifetimeManager());

            // surface
            container.RegisterType<IOidStrategy, ExternalOid>(new PerRequestLifetimeManager());
            container.RegisterType<INakedObjectsSurface, NakedObjectsSurface>(new PerRequestLifetimeManager());

            //Temporary scaffolding
            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerRequestLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerRequestLifetimeManager());

            //Externals
            container.RegisterType<IPrincipal>(new InjectionFactory(c => HttpContext.Current.User));

            //DI
            container.RegisterType<IFrameworkResolver, UnityFrameworkResolver>(new PerRequestLifetimeManager());

            // Facet decorators 
            if (NakedObjectsRunSettings.AuditConfig() != null) {
                container.RegisterType(typeof(IFacetDecorator), typeof(AuditManager), "AuditManager", new ContainerControlledLifetimeManager());
                container.RegisterInstance(typeof(IAuditConfiguration), NakedObjectsRunSettings.AuditConfig(), new ContainerControlledLifetimeManager());
            }

            if (NakedObjectsRunSettings.AuthorizationConfig() != null) {
                container.RegisterType(typeof(IFacetDecorator), typeof(AuthorizationManager), "AuthorizationManager", new ContainerControlledLifetimeManager());
                container.RegisterInstance(typeof(IAuthorizationConfiguration), NakedObjectsRunSettings.AuthorizationConfig(), new ContainerControlledLifetimeManager());
            }
        }

        #endregion

        #region Unity Container

        private static readonly Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() => {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer() {
            return container.Value;
        }

        #endregion
    }
}