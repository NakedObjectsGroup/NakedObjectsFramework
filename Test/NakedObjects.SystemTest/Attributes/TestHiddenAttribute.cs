// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Hidden {
        [TestClass]
        public class TestHiddenAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework(this);
                obj = NewTestObject<Object1>();
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
                obj = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>()
                                                              });
                }
            }
            #endregion

            private ITestObject obj;

            [TestMethod]
            public virtual void Hidden() {
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.AssertIsInvisible();

                obj.Save();
                prop1.AssertIsInvisible();
            }

            [TestMethod]
            public virtual void HiddenAlways() {
                ITestProperty prop5 = obj.GetPropertyByName("Prop5");
                prop5.AssertIsInvisible();
                obj.Save();
                prop5.AssertIsInvisible();
            }

            [TestMethod]
            public virtual void HiddenNever() {
                ITestProperty prop4 = obj.GetPropertyByName("Prop4");
                prop4.AssertIsVisible();
                obj.Save();
                prop4.AssertIsVisible();
            }

            [TestMethod]
            public virtual void HiddenOncePersisted() {
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertIsVisible();
                obj.Save();
                prop2.AssertIsInvisible();
            }

            [TestMethod]
            public virtual void HiddenUntilPersisted() {
                ITestProperty prop3 = obj.GetPropertyByName("Prop3");
                prop3.AssertIsInvisible();
                obj.Save();
                prop3.AssertIsVisible();
            }
        }

        public class Object1 {
            public Object1() {
                // initialise all fields 
                Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = string.Empty;
            }

            public string Prop0 { get; set; }

            [Hidden]
            public string Prop1 { get; set; }

            [Hidden(WhenTo.OncePersisted)]
            public string Prop2 { get; set; }

            [Hidden(WhenTo.UntilPersisted)]
            public string Prop3 { get; set; }

            [Hidden(WhenTo.Never)]
            public string Prop4 { get; set; }

            [Hidden(WhenTo.Always)]
            public string Prop5 { get; set; }
        }
    }
} //end of root namespace