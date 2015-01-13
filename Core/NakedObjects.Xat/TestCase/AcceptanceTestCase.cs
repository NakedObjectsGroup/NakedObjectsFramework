// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using Common.Logging;
using Microsoft.Practices.Unity;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Menu;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Adapter;
using NakedObjects.Core.Component;
using NakedObjects.Core.Configuration;
using NakedObjects.Core.Fixture;
using NakedObjects.Core.Spec;
using NakedObjects.Core.Util;
using NakedObjects.Meta;
using NakedObjects.Meta.Menu;
using NakedObjects.Persistor.Entity;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Reflect;
using NakedObjects.Reflect.FacetFactory;
using NakedObjects.Reflect.TypeFacetFactory;
using NakedObjects.Service;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NakedObjects.Xat {
    public abstract class AcceptanceTestCase {
        private static readonly ILog Log;
        private readonly Lazy<IUnityContainer> unityContainer;
        private FixtureServices fixtureServices;
        private INakedObjectsFramework nakedObjectsFramework;
        private IDictionary<string, ITestService> servicesCache = new Dictionary<string, ITestService>();
        private ITestObjectFactory testObjectFactory;
        private IPrincipal testPrincipal;
        private ISession testSession;

        static AcceptanceTestCase() {
            Log = LogManager.GetLogger(typeof (AcceptanceTestCase));
        }

        protected AcceptanceTestCase(string name) {
            Name = name;

            unityContainer = new Lazy<IUnityContainer>(() => {
                var c = new UnityContainer();
                RegisterTypes(c);
                return c;
            });
        }

        protected AcceptanceTestCase() : this("Unnamed") {}

        protected string Name { set; get; }

        protected virtual ITestObjectFactory TestObjectFactoryClass {
            get {
                
                if (testObjectFactory == null) {
                    testObjectFactory = new TestObjectFactory(NakedObjectsFramework.MetamodelManager, NakedObjectsFramework.Session, NakedObjectsFramework.LifecycleManager, NakedObjectsFramework.Persistor, NakedObjectsFramework.NakedObjectManager, NakedObjectsFramework.TransactionManager, NakedObjectsFramework.ServicesManager);
                }
                return testObjectFactory;
            }
        }

        protected virtual ISession TestSession {
            get {
                if (testSession == null) {
                    testSession = new TestSession(TestPrincipal);
                }
                return testSession;
            }
            set { testSession = value; }
        }

        protected virtual IPrincipal TestPrincipal {
            get {
                if (testPrincipal == null) {
                    testPrincipal = CreatePrincipal("Test", new string[] {});
                }
                return testPrincipal;
            }
            set { testPrincipal = value; }
        }

        [Obsolete("Use NakedObjectsFramework")]
        protected INakedObjectsFramework NakedObjectsContext {
            get { return nakedObjectsFramework; }
        }

        protected INakedObjectsFramework NakedObjectsFramework {
            get { return nakedObjectsFramework; }
        }

        protected virtual object[] Fixtures {
            get { return new object[] {}; }
        }

        protected virtual object[] MenuServices {
            get { return new object[] {}; }
        }

        protected virtual object[] ContributedActions {
            get { return new object[] {}; }
        }

        protected virtual object[] SystemServices {
            get { return new object[] {}; }
        }

        protected virtual Type[] Types {
            get { return new Type[] { }; }
        }

        protected void StartTest() {
            nakedObjectsFramework = GetConfiguredContainer().Resolve<INakedObjectsFramework>();
        }

        private void InstallFixtures(ITransactionManager transactionManager, IContainerInjector injector, object[] newFixtures) {
            foreach (object fixture in newFixtures) {
                InstallFixture(transactionManager, injector, fixture);
            }
        }

        private static void SetValue(PropertyInfo property, object injectee, object value) {
            if (property != null) {
                try {
                    property.SetValue(injectee, value, null);
                }
                catch (TargetInvocationException e) {
                    InvokeUtils.InvocationException("Exception executing " + property, e);
                }
            }
        }


        protected void PreInstallFixtures(ITransactionManager transactionManager) {
            fixtureServices = new FixtureServices();
        }


        private static MethodInfo GetInstallMethod(object fixture) {
            return fixture.GetType().GetMethod("Install", new Type[0]) ??
                   fixture.GetType().GetMethod("install", new Type[0]);
        }

        protected object[] GetFixtures(object fixture) {
            MethodInfo getFixturesMethod = fixture.GetType().GetMethod("GetFixtures", new Type[] {});
            return getFixturesMethod == null ? new object[] {} : (object[]) getFixturesMethod.Invoke(fixture, new object[] {});
        }

        protected void InstallFixture(object fixture) {
            PropertyInfo property = fixture.GetType().GetProperty("Service");
            SetValue(property, fixture, fixtureServices);

            MethodInfo installMethod = GetInstallMethod(fixture);
            Log.Debug("Invoking install method");
            try {
                installMethod.Invoke(fixture, new object[0]);
            }
            catch (TargetInvocationException e) {
                InvokeUtils.InvocationException("Exception executing " + installMethod, e);
            }
        }

        private void InstallFixture(ITransactionManager transactionManager, IContainerInjector injector, object fixture) {
            injector.InitDomainObject(fixture);

            // first, install any child fixtures (if this is a composite.
            object[] childFixtures = GetFixtures(fixture);
            InstallFixtures(transactionManager, injector, childFixtures);

            // now, install the fixture itself
            try {
                Log.Info("installing fixture: " + fixture);
                transactionManager.StartTransaction();
                InstallFixture(fixture);
                transactionManager.EndTransaction();
                Log.Info("fixture installed");
            }
            catch (Exception e) {
                Log.Error("installing fixture " + fixture.GetType().FullName + " failed (" + e.Message + "); aborting fixture ", e);
                try {
                    transactionManager.AbortTransaction();
                }
                catch (Exception e2) {
                    Log.Error("failure during abort", e2);
                }
                throw;
            }
        }

        protected void RunFixtures() {
            if (nakedObjectsFramework == null) {
                nakedObjectsFramework = GetConfiguredContainer().Resolve<INakedObjectsFramework>();
            }
            InstallFixtures(NakedObjectsFramework.TransactionManager, NakedObjectsFramework.ContainerInjector, Fixtures);
        }

        protected ITestService GetTestService(Type type) {
            return NakedObjectsFramework.ServicesManager.GetServices().
                Where(no => type.IsInstanceOfType(no.Object)).
                Select(no => TestObjectFactoryClass.CreateTestService(no.Object)).
                FirstOrDefault();
        }

        protected ITestService GetTestService(string serviceName) {
            if (!servicesCache.ContainsKey(serviceName.ToLower())) {
                foreach (INakedObject service in NakedObjectsFramework.ServicesManager.GetServices()) {
                    if (service.TitleString().Equals(serviceName, StringComparison.CurrentCultureIgnoreCase)) {
                        ITestService testService = TestObjectFactoryClass.CreateTestService(service.Object);
                        if (testService == null) {
                            Assert.Fail("Invalid service name " + serviceName);
                        }
                        servicesCache[serviceName.ToLower()] = testService;
                        return testService;
                    }
                }
                Assert.Fail("No such service: " + serviceName);

            }
            return servicesCache[serviceName.ToLower()];
        }

        protected ITestMenu GetMainMenu(string menuName) {
            var mainMenus = NakedObjectsFramework.MetamodelManager.MainMenus();
            if (mainMenus.Any()) {
                IMenuImmutable menu = mainMenus.FirstOrDefault(m => m.Name == menuName);
                if (menu == null) {
                    Assert.Fail("No such main menu " + menuName);
                }
                return TestObjectFactoryClass.CreateTestMenuMain(menu);
            } 
            
            //Use the MenuServices to derive the menus
            var service = GetTestService(menuName);
            if (service == null) {
                Assert.Fail("No such main menu, or Service, " + menuName);
            }
            return service.GetMenu();
        }

        protected ITestMenu[] AllMainMenus() {
            return NakedObjectsFramework.MetamodelManager.MainMenus().Select(m => TestObjectFactoryClass.CreateTestMenuMain(m)).ToArray();
        }

        protected void AssertMainMenuCountIs(int expected) {
            var actual = NakedObjectsFramework.MetamodelManager.MainMenus().Count();
            Assert.AreEqual(expected, actual);
        }

        protected ITestObject GetBoundedInstance<T>(string title) {
            return GetBoundedInstance(typeof (T), title);
        }

        protected ITestObject GetBoundedInstance(Type type, string title) {
            IObjectSpec spec = NakedObjectsFramework.MetamodelManager.GetSpecification(type);
            return GetBoundedInstance(title, spec);
        }

        protected ITestObject GetBoundedInstance(string classname, string title) {
            IObjectSpec spec = NakedObjectsFramework.MetamodelManager.GetSpecification(classname);
            return GetBoundedInstance(title, spec);
        }

        private ITestObject GetBoundedInstance(string title, IObjectSpec spec) {
            if (spec.GetFacet<IBoundedFacet>() == null) {
                Assert.Fail(spec.SingularName + " is not a Bounded type");
            }
            IEnumerable allInstances = NakedObjectsFramework.Persistor.Instances(spec);
            object inst = allInstances.Cast<object>().Single(o => NakedObjectsFramework.NakedObjectManager.CreateAdapter(o, null, null).TitleString() == title);
            return TestObjectFactoryClass.CreateTestObject(NakedObjectsFramework.NakedObjectManager.CreateAdapter(inst, null, null));
        }

        private static IPrincipal CreatePrincipal(string name, string[] roles) {
            return new GenericPrincipal(new GenericIdentity(name), roles);
        }

        protected void SetUser(string username, params string[] roles) {
            testPrincipal = CreatePrincipal(username, roles);
            var ts = TestSession as TestSession;
            if (ts != null) {
                ts.ReplacePrincipal(testPrincipal);
            }
        }

        protected void SetUser(string username) {
            SetUser(username, new string[] {});
        }

        protected static void InitializeNakedObjectsFramework(AcceptanceTestCase tc) {
            Log.Info("test initialize " + tc.Name);
            tc.servicesCache = new Dictionary<string, ITestService>();

            tc.GetConfiguredContainer().Resolve<IReflector>().Reflect();
        }

        protected static void CleanupNakedObjectsFramework(AcceptanceTestCase tc) {
            Log.Info("test cleanup " + tc.Name);
            Log.Info("cleanup " + tc.Name);

            tc.servicesCache.Clear();
            tc.servicesCache = null;
            tc.testObjectFactory = null;
        }

        protected virtual void RegisterFacetFactories(IUnityContainer container) {
            int order = 0;
            container.RegisterType<IFacetFactory, FallbackFacetFactory>("FallbackFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IteratorFilteringFacetFactory>("IteratorFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UnsupportedParameterTypesMethodFilteringFactory>("UnsupportedParameterTypesMethodFilteringFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveSuperclassMethodsFacetFactory>("RemoveSuperclassMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveInitMethodFacetFactory>("RemoveInitMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveDynamicProxyMethodsFacetFactory>("RemoveDynamicProxyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RemoveEventHandlerMethodsFacetFactory>("RemoveEventHandlerMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeMarkerFacetFactory>("TypeMarkerFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must be before any other FacetFactories that install MandatoryFacet.class facets
            container.RegisterType<IFacetFactory, MandatoryDefaultFacetFactory>("MandatoryDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyValidateDefaultFacetFactory>("PropertyValidateDefaultFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplementaryMethodsFilteringFacetFactory>("ComplementaryMethodsFilteringFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionMethodsFacetFactory>("ActionMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFieldMethodsFacetFactory>("CollectionFieldMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyMethodsFacetFactory>("PropertyMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IconMethodFacetFactory>("IconMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CallbackMethodsFacetFactory>("CallbackMethodsFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TitleMethodFacetFactory>("TitleMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateObjectFacetFactory>("ValidateObjectFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ComplexTypeAnnotationFacetFactory>("ComplexTypeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ViewModelFacetFactory>("ViewModelFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BoundedAnnotationFacetFactory>("BoundedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumFacetFactory>("EnumFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ActionDefaultAnnotationFacetFactory>("ActionDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PropertyDefaultAnnotationFacetFactory>("PropertyDefaultAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DescribedAsAnnotationFacetFactory>("DescribedAsAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisabledAnnotationFacetFactory>("DisabledAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PasswordAnnotationFacetFactory>("PasswordAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ExecutedAnnotationFacetFactory>("ExecutedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PotencyAnnotationFacetFactory>("PotencyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PageSizeAnnotationFacetFactory>("PageSizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenAnnotationFacetFactory>("HiddenAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, HiddenDefaultMethodFacetFactory>("HiddenDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DisableDefaultMethodFacetFactory>("DisableDefaultMethodFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, AuthorizeAnnotationFacetFactory>("AuthorizeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ValidateProgrammaticUpdatesAnnotationFacetFactory>("ValidateProgrammaticUpdatesAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImmutableAnnotationFacetFactory>("ImmutableAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MaxLengthAnnotationFacetFactory>("MaxLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RangeAnnotationFacetFactory>("RangeAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MemberOrderAnnotationFacetFactory>("MemberOrderAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, MultiLineAnnotationFacetFactory>("MultiLineAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NamedAnnotationFacetFactory>("NamedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, NotPersistedAnnotationFacetFactory>("NotPersistedAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ProgramPersistableOnlyAnnotationFacetFactory>("ProgramPersistableOnlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, OptionalAnnotationFacetFactory>("OptionalAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, RequiredAnnotationFacetFactory>("RequiredAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PluralAnnotationFacetFactory>("PluralAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DefaultNamingFacetFactory>("DefaultNamingFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); // must come after Named and Plural factories
            container.RegisterType<IFacetFactory, ConcurrencyCheckAnnotationFacetFactory>("ConcurrencyCheckAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ContributedActionAnnotationFacetFactory>("ContributedActionAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FinderActionFacetFactory>("FinderActionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles
            container.RegisterType<IFacetFactory, MaskAnnotationFacetFactory>("MaskAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            // must come after any facets that install titles, and after mask
            // if takes precedence over mask.
            container.RegisterType<IFacetFactory, RegExAnnotationFacetFactory>("RegExAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypeOfAnnotationFacetFactory>("TypeOfAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TableViewAnnotationFacetFactory>("TableViewAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthDerivedFromTypeFacetFactory>("TypicalLengthDerivedFromTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TypicalLengthAnnotationFacetFactory>("TypicalLengthAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EagerlyAnnotationFacetFactory>("EagerlyAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, PresentationHintAnnotationFacetFactory>("PresentationHintAnnotationFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, BooleanValueTypeFacetFactory>("BooleanValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ByteValueTypeFacetFactory>("ByteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, SbyteValueTypeFacetFactory>("SbyteValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ShortValueTypeFacetFactory>("ShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, IntValueTypeFacetFactory>("IntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, LongValueTypeFacetFactory>("LongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UShortValueTypeFacetFactory>("UShortValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, UIntValueTypeFacetFactory>("UIntValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ULongValueTypeFacetFactory>("ULongValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FloatValueTypeFacetFactory>("FloatValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DoubleValueTypeFacetFactory>("DoubleValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DecimalValueTypeFacetFactory>("DecimalValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CharValueTypeFacetFactory>("CharValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, DateTimeValueTypeFacetFactory>("DateTimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, TimeValueTypeFacetFactory>("TimeValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, StringValueTypeFacetFactory>("StringValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, GuidValueTypeFacetFactory>("GuidValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, EnumValueTypeFacetFactory>("EnumValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, FileAttachmentValueTypeFacetFactory>("FileAttachmentValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ImageValueTypeFacetFactory>("ImageValueTypeFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, ArrayValueTypeFacetFactory<byte>>("ArrayValueTypeFacetFactory<byte>", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++));
            container.RegisterType<IFacetFactory, CollectionFacetFactory>("CollectionFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); // written to not trample over TypeOf if already installed
            container.RegisterType<IFacetFactory, MenuFacetFactory>("MenuFacetFactory", new ContainerControlledLifetimeManager(), new InjectionConstructor(order++)); 
        }

        protected virtual void RegisterTypes(IUnityContainer container) {

            RegisterFacetFactories(container);
            container.RegisterType<IMenuFactory, MenuFactory>();

            container.RegisterType<ISpecificationCache, ImmutableInMemorySpecCache>(new ContainerControlledLifetimeManager(), new InjectionConstructor());
            container.RegisterType<IClassStrategy, DefaultClassStrategy>(new ContainerControlledLifetimeManager());

            container.RegisterType<IReflector, Reflector>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodel, Metamodel>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMetamodelBuilder, Metamodel>(new ContainerControlledLifetimeManager());

            container.RegisterType<IPrincipal>(new InjectionFactory(c => TestPrincipal));

            var config = new EntityObjectStoreConfiguration();

            //config.UsingEdmxContext("Model").AssociateTypes(AdventureWorksTypes);
            //config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] { typeof(AWDomainObject) });

            container.RegisterInstance<IEntityObjectStoreConfiguration>(config, (new ContainerControlledLifetimeManager()));

            // TODO still done for backward compatibility - 
            var reflectorConfig = new ReflectorConfiguration(
                Types ?? new Type[]{},
                MenuServices.Select(s => s.GetType()).ToArray(),
                ContributedActions.Select(s => s.GetType()).ToArray(),
                SystemServices.Select(s => s.GetType()).ToArray());

            container.RegisterInstance<IReflectorConfiguration>(reflectorConfig, (new ContainerControlledLifetimeManager()));

            container.RegisterType<NakedObjectFactory, NakedObjectFactory>(new PerResolveLifetimeManager());
            container.RegisterType<SpecFactory, SpecFactory>(new PerResolveLifetimeManager());

            container.RegisterType<IPocoAdapterMap, PocoAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));
            container.RegisterType<IIdentityAdapterMap, IdentityAdapterHashMap>(new PerResolveLifetimeManager(), new InjectionConstructor(10));

            container.RegisterType<IMessageBroker, MessageBroker>(new PerResolveLifetimeManager());
            container.RegisterType<IContainerInjector, DomainObjectContainerInjector>(new PerResolveLifetimeManager());

            container.RegisterType<IOidGenerator, EntityOidGenerator>(new PerResolveLifetimeManager());

            container.RegisterType<IObjectStore, EntityObjectStore>(new PerResolveLifetimeManager());
            container.RegisterType<IIdentityMap, IdentityMapImpl>(new PerResolveLifetimeManager());

            container.RegisterType<ITransactionManager, TransactionManager>(new PerResolveLifetimeManager());
            container.RegisterType<INakedObjectManager, NakedObjectManager>(new PerResolveLifetimeManager());
            container.RegisterType<IObjectPersistor, ObjectPersistor>(new PerResolveLifetimeManager());
            container.RegisterType<IServicesManager, ServicesManager>(new PerResolveLifetimeManager());
            container.RegisterType<IPersistAlgorithm, EntityPersistAlgorithm>(new PerResolveLifetimeManager());

            container.RegisterType<IMetamodelManager, MetamodelManager>(new PerResolveLifetimeManager());

            container.RegisterType<ILifecycleManager, LifeCycleManager>(new PerResolveLifetimeManager());

            container.RegisterType<ISession>(new PerResolveLifetimeManager(), new InjectionFactory(c => TestSession));

            container.RegisterType<IFrameworkResolver, UnityFrameworkResolver>(new PerResolveLifetimeManager());

            container.RegisterType<INakedObjectsFramework, NakedObjectsFramework>(new PerResolveLifetimeManager());
        }

        /// <summary>
        ///     Gets the configured Unity unityContainer.
        /// </summary>
        protected IUnityContainer GetConfiguredContainer() {
            return unityContainer.Value;
        }
    }

    // Copyright (c) INakedObject Objects Group Ltd.
}