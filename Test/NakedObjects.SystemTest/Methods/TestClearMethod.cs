// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace Clear {
        [TestClass]
        public class TestClearMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework();
                //Set up any common variables here
            }

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework();
                //Tear down any common variables here
            }

            #endregion

            #region "Services & Fixtures"
            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>(),
                                                                  new SimpleRepository<Object3>()
                                                              });
                }
            }

            //Specify as you would for the run class in a prototype
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }
            #endregion


            private void SetupForClearTests(out ITestProperty prop0, out ITestProperty prop1) {
                ITestObject obj = NewTestObject<Object1>();
                prop0 = obj.GetPropertyByName("Prop0");
                prop1 = obj.GetPropertyByName("Prop1");

                prop0.AssertIsEmpty();
                prop1.AssertIsEmpty();

                prop1.SetValue("Foo");
                prop1.AssertValueIsEqual("Foo");
                prop0.AssertIsEmpty();
            }

            private static void CheckForClearTests(ITestProperty prop0, ITestProperty prop1) {
                prop1.AssertIsEmpty();
                prop0.AssertValueIsEqual("Prop1 has been cleared");
            }

            [TestMethod]
            public void ClearMethodDoesNotShowUpAsAnAction() {
                ITestObject obj1 = NewTestObject<Object1>();
                try {
                    obj1.GetAction("Clear Prop1");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public void ClearMethodOnReferenceProperty() {
                ITestObject obj3 = NewTestObject<Object3>();
                obj3.GetPropertyByName("Prop1").SetValue("Foo");
                obj3.Save();

                ITestObject obj1 = NewTestObject<Object1>();
                ITestProperty prop3 = obj1.GetPropertyByName("Prop3");
                ITestProperty prop4 = obj1.GetPropertyByName("Prop4");

                prop3.AssertIsEmpty();
                prop4.AssertIsEmpty();

                prop4.SetObject(obj3);
                prop4.AssertObjectIsEqual(obj3);
                prop3.AssertIsEmpty();

                prop4.ClearObject();
                prop3.AssertValueIsEqual("Prop4 has been cleared");
            }

            [TestMethod]
            public void ClearMethodOnValueProperty() {
                ITestProperty prop0, prop1;
                SetupForClearTests(out prop0, out prop1);
                prop1.ClearValue();
                CheckForClearTests(prop0, prop1);
            }

            [TestMethod]
            public void SetToEmptyStringOnValueProperty() {
                ITestProperty prop0, prop1;
                SetupForClearTests(out prop0, out prop1);
                prop1.SetValue("");
                CheckForClearTests(prop0, prop1);
            }

            [TestMethod]
            public void UnmatchedModifyMethodShowsUpAsAnAction() {
                ITestObject obj2 = NewTestObject<Object2>();
                obj2.GetAction("Clear Prop2");
                obj2.GetAction("Clear Prop3");
                obj2.GetAction("Clear Prop4");
            }
        }

        public class Object1 {
            public string Prop0 { get; set; }

            [Optionally]
            public string Prop1 { get; set; }

            public string Prop3 { get; set; }

            public Object3 Prop4 { get; set; }

            public void ClearProp1() {
                Prop1 = null;
                Prop0 = "Prop1 has been cleared";
            }

            public void ClearProp4() {
                Prop4 = null;
                Prop3 = "Prop4 has been cleared";
            }
        }

        public class Object2 {
            public string Prop2 { get; set; }

            //Has a parameter

            public string Prop3 { get; set; }
            public void ClearProp2(string value) {}

            //Non-void method
            public bool ClearProp3() {
                return false;
            }

            //No corresponding Prop4
            public void ClearProp4() {}
        }

        public class Object3 {
            [Title]
            public string Prop1 { get; set; }
        }
    }
}