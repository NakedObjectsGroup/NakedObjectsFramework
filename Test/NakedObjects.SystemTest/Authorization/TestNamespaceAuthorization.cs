// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyApp.MyCluster1;
using MyApp.MyCluster2;
using NakedObjects;
using NakedObjects.Security;
using NakedObjects.Services;
using NotMyApp.MyCluster2;

namespace NakedObjects.SystemTest.Authorization.NamespaceAuthorization {
    [TestClass, Ignore]
    public class TestNamespaceAuthorization : AbstractSystemTest<NamespaceAuthorizationDbContext> {
        #region Setup/Teardown

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc) {
            InitializeNakedObjectsFramework(new TestNamespaceAuthorization());
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestNamespaceAuthorization());
            Database.Delete(NamespaceAuthorizationDbContext.DatabaseName);
        }

        [TestInitialize()]
        public void TestInitialize() {
            StartTest();
            SetUser("sven");
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        #region Services & Fixtures

        protected override object[] MenuServices {
            get {
                return new object[] {
                    new SimpleRepository<Foo1>(),
                    new SimpleRepository<Bar1>(),
                    new SimpleRepository<Foo2>(),
                    new SimpleRepository<Bar2>()
                };
            }
        }

        #endregion

        //protected override IAuthorizerInstaller Authorizer
        //{
        //    get
        //    {
        //        return new CustomAuthorizerInstaller(
        //            new MyDefaultAuthorizer(),
        //            new MyAppAuthorizer(),
        //            new MyCluster1Authorizer(),
        //            new MyBar1Authorizer()
        //            );
        //    }
        //}

        [TestMethod]
        public void AuthorizerWithMostSpecificNamespaceIsInvokedForVisibility() {
            //Bar1
            var bar1 = GetTestService("Bar1s").GetAction("New Instance").InvokeReturnObject();
            try {
                bar1.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("MyBar1Authorizer#IsVisible, user: sven, target: Bar1, memberName: Prop1", e.InnerException.Message);
            }

            //Foo1
            var foo1 = GetTestService("Foo1s").GetAction("New Instance").InvokeReturnObject();
            try {
                foo1.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("MyCluster1Authorizer#IsVisible, user: sven, target: Foo1, memberName: Prop1", e.InnerException.Message);
            }

            //Foo2
            var foo2 = GetTestService("Foo2s").GetAction("New Instance").InvokeReturnObject();
            try {
                foo2.GetPropertyByName("Prop1").AssertIsVisible();
                Assert.Fail("Should not get to here");
            }
            catch (Exception e) {
                Assert.AreEqual("MyAppAuthorizer#IsVisible, user: sven, target: Foo2, memberName: Prop1", e.InnerException.Message);
            }

            //Bar2
            var bar2 = GetTestService("Bar2s").GetAction("New Instance").InvokeReturnObject();
            bar2.GetPropertyByName("Prop1").AssertIsVisible();
        }
    }

    #region Classes used by tests 

    public class NamespaceAuthorizationDbContext : DbContext {
        public const string DatabaseName = "TestAttributes";
        public NamespaceAuthorizationDbContext() : base(DatabaseName) {}

        public DbSet<Foo1> Foo1s { get; set; }
        public DbSet<Bar1> Bar1s { get; set; }
        public DbSet<Foo2> Foo2s { get; set; }
        public DbSet<Bar2> Bar2s { get; set; }
    }

    public class MyDefaultAuthorizer : ITypeAuthorizer<object> {
        //bool initialized = false;

        #region ITypeAuthorizer<object> Members

        public void Init() {
            //initialized = false;
            throw new NotImplementedException();
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            return true;
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MyAppAuthorizer : INamespaceAuthorizer {
        #region INamespaceAuthorizer Members

        public string NamespaceToAuthorize {
            get { return "MyApp"; }
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyAppAuthorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyAppAuthorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        #endregion

        public void Init() {}

        public void Shutdown() {
            throw new NotImplementedException();
        }
    }

    public class MyCluster1Authorizer : INamespaceAuthorizer {
        #region INamespaceAuthorizer Members

        public string NamespaceToAuthorize {
            get { return "MyApp.MyCluster1"; }
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyCluster1Authorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyCluster1Authorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        #endregion

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }
    }

    public class MyBar1Authorizer : INamespaceAuthorizer {
        #region INamespaceAuthorizer Members

        public string NamespaceToAuthorize {
            get { return "MyApp.MyCluster1.Bar1"; }
        }

        public bool IsEditable(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyBar1Authorizer#IsEditable, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        public bool IsVisible(IPrincipal principal, object target, string memberName) {
            throw new Exception(String.Format("MyBar1Authorizer#IsVisible, user: {0}, target: {1}, memberName: {2}", principal.Identity.Name, target.ToString(), memberName));
        }

        #endregion

        public void Init() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }
    }
}

namespace MyApp.MyCluster1 {
    public class Foo1 {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Foo1";
        }

        public void Act1() {}
    }

    public class Bar1 {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Bar1";
        }

        public void Act1() {}
    }
}

namespace MyApp.MyCluster2 {
    public class Foo2 {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Foo2";
        }

        public void Act1() {}
    }
}

namespace NotMyApp.MyCluster2 {
    public class Bar2 {
        [Optionally]
        public virtual string Prop1 { get; set; }

        public override string ToString() {
            return "Bar2";
        }

        public void Act1() {}
    }

    #endregion
}