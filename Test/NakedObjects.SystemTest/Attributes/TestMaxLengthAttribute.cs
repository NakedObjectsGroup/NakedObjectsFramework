// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Attributes {
    namespace MaxLength {
     
        [TestClass]
        public class TestMaxLengthAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize]
            public void SetupTest() {
                InitializeNakedObjectsFramework();
                base.StartMethodProfiling();
            }

            [TestCleanup]
            public void TearDownTest() {
                CleanupNakedObjectsFramework();
                base.StopMethodProfiling();
            }

            #endregion

            #region "Services & Fixtures"

            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get { return new ServicesInstaller(new object[] {new SimpleRepository<Object1>(), new SimpleRepository<Object2>()}); }
            }

            #endregion

            [TestMethod]
            public virtual void NakedObjectsMaxLengthOnProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertFieldEntryInvalid("12345678");
                prop2.AssertFieldEntryInvalid("1234567 ");
                prop2.SetValue("1234567");
            }

            [TestMethod]
            public virtual void NakedObjectsMaxLengthOnParm() {
                ITestObject obj = NewTestObject<Object1>();
                ITestAction act = obj.GetAction("Action");

                act.AssertIsInvalidWithParms("123456789");
                act.AssertIsInvalidWithParms("12345678 ");
                act.AssertIsValidWithParms("12345678");
            }

            [TestMethod]
            public virtual void ComponentModelMaxLengthOnProperty() {
                ITestObject obj = NewTestObject<Object2>();
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertFieldEntryInvalid("12345678");
                prop2.AssertFieldEntryInvalid("1234567 ");
                prop2.SetValue("1234567");
            }

            [TestMethod]
            public virtual void ComponentModelMaxLengthOnParm() {
                ITestObject obj = NewTestObject<Object2>();
                ITestAction act = obj.GetAction("Action");

                act.AssertIsInvalidWithParms("123456789");
                act.AssertIsInvalidWithParms("12345678 ");
                act.AssertIsValidWithParms("12345678");
            }
        }

        public class Object1 {
            public string Prop1 { get; set; }


            [MaxLength(7)]
            public string Prop2 { get; set; }


            public void Action([MaxLength(8)] string parm) {}
        }

        public class Object2 {
            public string Prop1 { get; set; }

            [MaxLength(7)]
            public string Prop2 { get; set; }

            public void Action([MaxLength(8)] string parm) {}
        }
    }
}

//end of root namespace