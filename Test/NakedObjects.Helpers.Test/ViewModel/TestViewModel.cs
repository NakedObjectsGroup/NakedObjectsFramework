// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel;
using System.Data.Entity;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.EntityObjectStore;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.Helpers.Test.ViewModel {
    public class DatabaseInitializer : DropCreateDatabaseAlways<FooContext> {}

    [TestClass]
    public class TestViewModel : AcceptanceTestCase {
        private ITestObject foo1;

        #region Constructors

        public TestViewModel(string name) : base(name) {}

        public TestViewModel() : this(typeof (TestViewModel).Name) {}

        #endregion

        #region Run configuration

        //Set up the properties in this region exactly the same way as in your Run class

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>(), new ViewModelService()}); }
        }

        protected override IServicesInstaller ContributedActions {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller SystemServices {
            get { return new ServicesInstaller(new object[] {}); }
        }

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        #endregion

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            var config = new EntityObjectStoreConfiguration {EnforceProxies = false};
            config.UsingCodeFirstContext(() => new FooContext("HelpersTest"));
            container.RegisterInstance(config, (new ContainerControlledLifetimeManager()));
        }

        [ClassInitialize]
        public static void SetupTestFixture(TestContext tc) {
            Database.SetInitializer(new DatabaseInitializer());
            var test = new TestViewModel();
            InitializeNakedObjectsFramework(test);
            //test.RunFixtures();
        }

        [ClassCleanup]
        public static void TearDownTest() {
            CleanupNakedObjectsFramework(new TestViewModel());
            //Database.Delete("HelpersTest");
        }

        [TestInitialize]
        public void Initialize() {
            StartTest();
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            foo1.GetPropertyByName("Id").SetValue("12345");
            foo1.GetPropertyByName("Name").SetValue("Foo1");
            foo1.Save();
        }

        [TestCleanup]
        public void Cleanup() {
            foo1 = null;
        }

        [TestMethod]
        public virtual void ViewModelDerivesKeyFromRoot() {
            ITestObject vm = foo1.GetAction("View Model").InvokeReturnObject();
            vm.AssertIsType(typeof (ViewFoo));
            vm.AssertCannotBeSaved();
            vm.GetPropertyByName("Key").AssertTitleIsEqual("12345");
        }

        [TestMethod]
        public virtual void ViewModelRestoresRootGivenKey() {
            ITestObject view1 = GetTestService("Views").GetAction("New View Foo").InvokeReturnObject("12345");
            ITestProperty root = view1.GetPropertyByName("Root");
            root.AssertTitleIsEqual("Foo1");
        }

        [TestMethod]
        public virtual void AttemptRestoreWithInvalidKey() {
            try {
                ITestObject view1 = GetTestService("Views").GetAction("New View Foo").InvokeReturnObject("54321");
                Assert.Fail("Test should not get to this line");
            }
            catch (Exception e) {
                Assert.IsInstanceOfType(e.InnerException, typeof (DomainException));
                Assert.AreEqual("No instance with Id: 54321", e.Message);
            }
        }
    }

    public class FooContext : DbContext {
        public FooContext(string name) : base(name) {}

        public DbSet<Foo> Foos { get; set; }
    }


    //Persistent object on which ViewModel is based
    public class Foo : IHasIntegerId {
        public IDomainObjectContainer Container { set; protected get; }


        [Title]
        public virtual string Name { get; set; }

        #region IHasIntegerId Members

        public virtual int Id { get; set; }

        #endregion

        public ViewFoo ViewModel() {
            var vm = Container.NewViewModel<ViewFoo>();
            vm.Root = this;
            return vm;
        }
    }

    public class ViewFoo : ViewModel<Foo> {
        //For testing only
        public string Key {
            get { return DeriveKeys()[0]; }
        }

        public override bool HideRoot() {
            return false;
        }
    }

    [DisplayName("Views")]
    public class ViewModelService {
        public IDomainObjectContainer Container { set; protected get; }

        public ViewFoo NewViewFoo(string key) {
            var vm = Container.NewViewModel<ViewFoo>();
            vm.PopulateUsingKeys(new[] {key});
            return vm;
        }
    }
}