// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Named {
        [TestClass]
        public class TestNamedAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework();
                base.StartMethodProfiling();
                obj = NewTestObject<Object1>();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework();
                base.StopMethodProfiling();
                obj = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object1>()}); }
            }
#endregion

            private ITestObject obj;

            [TestMethod]
            public virtual void NamedAppliedToAction() {
                ITestAction hex = obj.GetAction("Hex");
                Assert.IsNotNull(hex);
                StringAssert.Equals("Hex", hex.Name);
            }

            [TestMethod]
            public virtual void NamedAppliedToObject() {
                obj.AssertTitleEquals("Untitled Foo");
            }

            [TestMethod]
            public virtual void NamedAppliedToParameter() {
                ITestAction hex = obj.GetAction("Hex");
                ITestParameter param = hex.Parameters[0];
                StringAssert.Equals("Yop", param.Name);
            }

            [TestMethod]
            public virtual void NamedAppliedToProperty() {
                ITestProperty bar = obj.GetPropertyByName("Bar");
                Assert.IsNotNull(bar);
            }
        }

        [Named("Foo")]
        public class Object1 {
            [Named("Bar")]
            public string Prop1 { get; set; }

            [Named("Hex")]
            public void DoSomething([Named("Yop")] string param1) {}
        }
    }
} //end of root namespace