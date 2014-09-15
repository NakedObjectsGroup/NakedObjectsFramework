// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace ToString {
        [TestClass, Ignore]
        public class TestToString : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void Setup() {
                InitializeNakedObjectsFramework(this);
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
                rep = null;
                obj = null;
                prop1 = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object2>()}); }
            }
            #endregion

            private ITestService rep;
            private ITestObject obj;
            private ITestProperty prop1;

          

            [TestMethod]
            public virtual void ToStringRecognisedAsATitle() {
                rep = GetTestService("Object2s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Bar");
                obj.AssertTitleEquals("Bar");
                obj.Save();
                obj.AssertTitleEquals("Bar");
            }
        }

        public class Object2 {
            [Optionally]
            public string Prop1 { get; set; }

            public override string ToString() {
                return Prop1;
            }
        }
    }
} //end of root namespace