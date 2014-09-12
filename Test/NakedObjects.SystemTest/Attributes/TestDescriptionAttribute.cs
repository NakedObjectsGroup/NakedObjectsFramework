// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Description {
        [TestClass]
        public class TestDescriptionAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework(this);
                
                obj1 = NewTestObject<Object1>();
                obj2 = NewTestObject<Object2>();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework(this);
                
                obj1 = null;
                obj2 = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] { }); }
            }

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {new SimpleRepository<Object1>(),
                                                               new SimpleRepository<Object2>()});
                }
            }
            #endregion

            private ITestObject obj1;
            private ITestObject obj2;

            [TestMethod]
            public virtual void DescribedAsAppliedToAction() {
                ITestAction action = obj1.GetAction("Do Something");
                Assert.IsNotNull(action);
                action.AssertIsDescribedAs("Hex");
            }

            [TestMethod]
            public virtual void DescribedAsAppliedToObject() {
                obj1.AssertIsDescribedAs("Foo");
            }

            [TestMethod]
            public virtual void DescribedAsAppliedToParameter() {
                ITestAction action = obj1.GetAction("Do Something");
                ITestParameter param = action.Parameters[0];
                param.AssertIsDescribedAs("Yop");
            }

            [TestMethod]
            public virtual void DescribedAsAppliedToProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop1");
                prop.AssertIsDescribedAs("Bar");
            }

            [TestMethod]
            public virtual void NullDescribedAsAppliedToAction() {
                ITestAction action = obj2.GetAction("Do Something");
                Assert.IsNotNull(action);
                action.AssertIsDescribedAs("");
            }

            [TestMethod]
            public virtual void NullDescribedAsAppliedToObject() {
                obj2.AssertIsDescribedAs("");
            }

            [TestMethod]
            public virtual void NullDescribedAsAppliedToParameter() {
                ITestAction action = obj2.GetAction("Do Something");
                ITestParameter param = action.Parameters[0];
                param.AssertIsDescribedAs("");
            }

            [TestMethod]
            public virtual void NullDescribedAsAppliedToProperty() {
                ITestProperty prop = obj2.GetPropertyByName("Prop1");
                prop.AssertIsDescribedAs("");
            }

        }

        [System.ComponentModel.Description("Foo")]
        public class Object1 {
            [System.ComponentModel.Description("Bar")]
            public string Prop1 { get; set; }

            [System.ComponentModel.Description("Hex")]
            public void DoSomething([System.ComponentModel.Description("Yop")] string param1) { }
        }


        public class Object2 {
            public string Prop1 { get; set; }
            public void DoSomething(string param1) { }
        }

    }
} //end of root namespace