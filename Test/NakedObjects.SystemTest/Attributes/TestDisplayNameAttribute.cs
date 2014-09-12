// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.ComponentModel;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace DisplayName {
        [TestClass]
        public class TestDisplayNameAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework(this);
                
                obj = NewTestObject<Object1>();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework(this);
                
                obj = null;
            }

            #endregion

            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object1>()}); }
            }

            private ITestObject obj;

            [TestMethod]
            public virtual void DisplayNameAppliedToAction() {
                ITestAction hex = obj.GetAction("Hex");
                Assert.IsNotNull(hex);
                StringAssert.Equals("Hex", hex.Name);
            }

            [TestMethod]
            public virtual void DisplayNameAppliedToObject() {
                obj.AssertTitleEquals("Untitled Foo");
            }
        }

        [DisplayName("Foo")]
        public class Object1 {
            [DisplayName("Bar")]
            public string Prop1 { get; set; }

            [DisplayName("Hex")]
            public void DoSomething(string param1) {}
        }
    }
}

//end of root namespace