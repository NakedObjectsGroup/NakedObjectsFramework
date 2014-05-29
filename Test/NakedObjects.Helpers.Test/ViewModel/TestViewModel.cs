// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.Helpers.Test.ViewModel {
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

        //protected override IObjectPersistorInstaller Persistor
        //{
        //    get { return new EntityPersistorInstaller(); }
        //}

        #endregion

        #region Initialize and Cleanup

        [TestInitialize]
        public void Initialize() {
            InitializeNakedObjectsFramework();
            // Use e.g. DatabaseUtils.RestoreDatabase to revert database before each test (or within a [ClassInitialize()] method).
            foo1 = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            foo1.GetPropertyByName("Id").SetValue("12345");
            foo1.GetPropertyByName("Name").SetValue("Foo1");
            foo1.Save();
        }

        [TestCleanup]
        public void Cleanup() {
            CleanupNakedObjectsFramework();
            foo1 = null;
        }

        #endregion

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

    //Persistent object on which ViewModel is based
    public class Foo : IHasIntegerId {
        public IDomainObjectContainer Container { set; protected get; }


        [Title]
        public virtual string Name { get; set; }

        public virtual int Id { get; set; }

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