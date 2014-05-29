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
    namespace Modify {
        [TestClass]
        public class TestModifyMethod : AbstractSystemTest {
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
                                                                  new SimpleRepository<Object3>(),
                                                                  new SimpleRepository<Object4>()
                                                              });
                }
            }

            //Specify as you would for the run class in a prototype
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }
            #endregion

            [TestMethod]
            public void ModifyMethodDoesNotShowUpAsAnAction() {
                ITestObject obj1 = NewTestObject<Object1>();
                try {
                    obj1.GetAction("Modify Prop1");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public void ModifyMethodOnReferenceProperty() {
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
                prop3.AssertValueIsEqual("Prop4 has been modified");
            }

            [TestMethod]
            public void ModifyMethodOnValueProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop0 = obj.GetPropertyByName("Prop0");
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");

                prop0.AssertIsEmpty();
                prop1.AssertIsEmpty();

                prop1.SetValue("Foo");
                prop1.AssertValueIsEqual("Foo");
                prop0.AssertValueIsEqual("Prop1 has been modified");
            }

            [TestMethod]
            public void CalledWhenReferencePropertyCleared() {
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
                prop3.SetValue("Neutral");

                prop4.ClearObject();
                prop4.AssertIsEmpty();
                prop3.AssertValueIsEqual("Prop4 has been modified");
            }

            [TestMethod]
            public void CalledWhenValuePropertyIsCleared() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop0 = obj.GetPropertyByName("Prop0");
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");

                prop1.SetValue("Foo");
                prop0.SetValue("Foo");

                prop1.ClearValue();
                prop1.AssertIsEmpty();
                prop0.AssertValueIsEqual("Prop1 has been modified");
            }

            [TestMethod]
            public void NotCalledWhenReferencePropertyClearedIfClear() {
                ITestObject obj3 = NewTestObject<Object3>();
                obj3.GetPropertyByName("Prop1").SetValue("Foo");
                obj3.Save();

                ITestObject obj1 = NewTestObject<Object4>();
                ITestProperty prop3 = obj1.GetPropertyByName("Prop3");
                ITestProperty prop4 = obj1.GetPropertyByName("Prop4");

                prop3.AssertIsEmpty();
                prop4.AssertIsEmpty();

                prop4.SetObject(obj3);
                prop4.AssertObjectIsEqual(obj3);
                prop3.SetValue("Neutral");

                prop4.ClearObject();
                prop4.AssertIsEmpty();
                prop3.AssertValueIsEqual("Neutral");
            }

            [TestMethod]
            public void NotCalledWhenValuePropertyIsClearedIfClear() {
                ITestObject obj = NewTestObject<Object4>();
                ITestProperty prop0 = obj.GetPropertyByName("Prop0");
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");

                prop1.SetValue("Foo");
                prop0.SetValue("Foo");

                prop1.ClearValue();
                prop1.AssertIsEmpty();
                prop0.AssertValueIsEqual("Foo");
            }

            [TestMethod]
            public void UnmatchedModifyMethodShowsUpAsAnAction() {
                ITestObject obj2 = NewTestObject<Object2>();
                obj2.GetAction("Modify Prop2");
                obj2.GetAction("Modify Prop3");
                obj2.GetAction("Modify Prop4");
            }
        }

        public class Object1 {
            public string Prop0 { get; set; }

            [Optionally]
            public string Prop1 { get; set; }

            public string Prop3 { get; set; }

            public Object3 Prop4 { get; set; }

            public void ModifyProp1(string value) {
                Prop1 = value;
                Prop0 = "Prop1 has been modified";
            }

            public void ModifyProp4(Object3 value) {
                Prop4 = value;
                Prop3 = "Prop4 has been modified";
            }
        }

        public class Object2 {
            public string Prop2 { get; set; }

            //Has the wrong type of parameter

            public string Prop3 { get; set; }
            public void ModifyProp2(int value) {}

            //Non-void method
            public bool ModifyProp3(string value) {
                return false;
            }

            //No corresponding Prop4
            public void ModifyProp4(string value) {}
        }

        public class Object3 {
            [Title]
            public string Prop1 { get; set; }
        }

        public class Object4 {
            public string Prop0 { get; set; }

            [Optionally]
            public string Prop1 { get; set; }

            public string Prop3 { get; set; }

            public Object3 Prop4 { get; set; }

            public void ModifyProp1(string value) {
                Prop1 = value;
                Prop0 = "Prop1 has been modified";
            }

            public void ModifyProp4(Object3 value) {
                Prop4 = value;
                Prop3 = "Prop4 has been modified";
            }

            public void ClearProp1() {
                Prop1 = null;

            }

            public void ClearProp4() {
                Prop4 = null;

            }
        }
    }
}