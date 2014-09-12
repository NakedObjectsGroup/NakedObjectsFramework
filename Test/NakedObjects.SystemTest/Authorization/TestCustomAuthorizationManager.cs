// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Persistor.Objectstore.Inmemory;
using NakedObjects.Reflector.Security;
using NakedObjects.Security;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Authorization.CustomAuthorizer {
    [TestClass]
    public class TestCustomAuthorizationManager : AbstractSystemTest {
        #region Setup/Teardown

        [TestInitialize]
        public void SetupTest() {
            InitializeNakedObjectsFramework(this);
            SetUser("sven");
        }

        [TestCleanup]
        public void TearDownTest() {
            CleanupNakedObjectsFramework(this);
            MemoryObjectStore.DiscardObjects();
        }

        #endregion

        #region "Services & Fixtures"

        protected override IFixturesInstaller Fixtures {
            get { return new FixturesInstaller(new object[] {}); }
        }

        protected override IServicesInstaller MenuServices {
            get { return new ServicesInstaller(new object[] {new SimpleRepository<Foo>(), new SimpleRepository<Bar>(), new SimpleRepository<FooSub>(), new SimpleRepository<Qux>()}); }
        }


        //protected override IAuthorizerInstaller Authorizer {
        //    get { return new CustomAuthorizerInstaller( new MyDefaultAuthorizer(), new FooAuthorizer(), new QuxAuthorizer()); }
        //}

        #endregion

        [TestMethod]
        public void VisibilityUsingSpecificTypeAuthorizer() {

           

            ITestObject foo = GetTestService("Foos").GetAction("New Instance").InvokeReturnObject();
            try {
                foo.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("FooAuthorizer#IsVisible, user: sven, target: foo1, memberName: Prop1", e.InnerException.Message);
            }
        }


        [TestMethod]
        public void EditabilityUsingSpecificTypeAuthorizer() {
            ITestObject qux = GetTestService("Quxes").GetAction("New Instance").InvokeReturnObject();
            try {
                qux.GetPropertyByName("Prop1").AssertIsModifiable();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Prop1", e.InnerException.Message);
            }
        }

        [TestMethod]
        public void DefaultAuthorizerCalledForNonSpecificType() {
            ITestObject bar1 = GetTestService("Bars").GetAction("New Instance").InvokeReturnObject();
            ITestProperty prop1 = bar1.GetPropertyByName("Prop1");
            prop1.AssertIsVisible();
            prop1.AssertIsModifiable();
        }

        [TestMethod]
        public void SubClassIsNotPickedUpByTypeAuthorizer() {
            ITestObject fooSub = GetTestService("Foo Subs").GetAction("New Instance").InvokeReturnObject();
            ITestProperty prop1 = fooSub.GetPropertyByName("Prop1");
            prop1.AssertIsVisible();
            prop1.AssertIsModifiable();
        }
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
        public IDomainObjectContainer Container { get; set; }
        public SimpleRepository<Foo> Service { get; set; }

     

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            return true;
        }

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            //Does nothing
        }
    }


    public class FooAuthorizer : ITypeAuthorizer<Foo> {
        public IDomainObjectContainer Container { get; set; }
        public SimpleRepository<Foo> Service { get; set; }

        public void Init() {
            //Does nothing
        }

        public bool IsEditable(IPrincipal principal, Foo target, string memberName) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            throw new NotImplementedException();
        }

        public bool IsVisible(IPrincipal principal, Foo target, string memberName) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            throw new Exception(String.Format("FooAuthorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target, memberName));
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public class QuxAuthorizer : ITypeAuthorizer<Qux> {
        public IDomainObjectContainer Container { get; set; }
        public SimpleRepository<Foo> Service { get; set; }

        public void Init() {
            //Does nothing
        }

        //"QuxAuthorizer#IsEditable, user: sven, target: qux1, memberName: Prop1"
        public bool IsEditable(IPrincipal principal, Qux target, string memberName) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            throw new Exception(String.Format("QuxAuthorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target, memberName));
        }

        public bool IsVisible(IPrincipal principal, Qux target, string memberName) {
            Assert.IsNotNull(Container);
            Assert.IsNotNull(Service);
            return true;
        }

        public void Shutdown() {
            //Does nothing
        }
    }

    public class Foo {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "foo1";
        }
    }

    public class Qux {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "qux1";
        }
    }

    public class Bar {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public void Act1() {}
    }

    public class FooSub : Foo {
        [Optionally]
        public virtual string Prop2 { get; set; }
    }
}