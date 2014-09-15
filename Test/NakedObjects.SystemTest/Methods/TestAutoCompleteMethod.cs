// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;

namespace NakedObjects.SystemTest.Methods {
    namespace AutoComplete {
        [TestClass, Ignore]
        public class TestAutoCompleteMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize]
            public void SetUp() {
                InitializeNakedObjectsFramework(this);
                //Set up any common variables here
                obj1 = NewTestObject<Object1>();
                obj2 = NewTestObject<Object2>();
                obj2.GetPropertyByName("Prop1").SetValue("Bar1");
                obj2.Save();
                obj2 = NewTestObject<Object2>();
                obj2.GetPropertyByName("Prop1").SetValue("Bar2");
                obj2.Save();
                obj2 = NewTestObject<Object2>();
                obj2.GetPropertyByName("Prop1").SetValue("Bar3");
                obj2.Save();
            }

            [TestCleanup]
            public void TearDown() {
                CleanupNakedObjectsFramework(this);
                obj1 = null;
                obj2 = null;
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

            private ITestObject obj1;
            private ITestObject obj2;

            [TestMethod]
            public void AutoCompleteMethodDoesNotShowUpAsAction() {
                try {
                    obj1.GetAction("Auto Complete Prop1");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }

                try {
                    obj1.GetAction("Auto Complete 0 Do Something");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }


            [TestMethod]
            public virtual void AutoCompleteParameters() {
                ITestAction action = obj1.GetAction("Do Something");

                try {
                    action.Parameters[0].GetCompletions("any");
                    Assert.Fail("expect exception");
                }
                catch (Exception) {
                    // expected
                }

                ITestNaked[] cho1 = action.Parameters[1].GetCompletions("any");
                Assert.AreEqual(3, cho1.Count());
                Assert.AreEqual("Fee", cho1[0].Title);

                ITestNaked[] cho2 = action.Parameters[2].GetCompletions("any");
                Assert.AreEqual(3, cho2.Count());
                Assert.AreEqual("Bar1", cho2[0].Title);
            }

            [TestMethod]
            public virtual void AutoCompleteReferenceProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop3");
                ITestNaked[] cho = prop.GetCompletions("any");
                Assert.AreEqual(3, cho.Count());
                Assert.AreEqual("Bar1", cho[0].Title);
            }

            [TestMethod]
            public virtual void AutoCompleteStringProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop2");
                ITestNaked[] cho = prop.GetCompletions("any");
                Assert.AreEqual(3, cho.Count());
                Assert.AreEqual("Fee", cho[0].Title);
            }

            [TestMethod]
            public virtual void AutoCompleteIntProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop1");

                try {
                    prop.GetCompletions("any");
                    Assert.Fail("expect exception");
                }
                catch (Exception ) {
                    // expected
                }
                obj1.GetAction("Auto Complete Prop1");
            }


            [TestMethod]
            public void UnmatchedAutoCompleteMethodShowsUpAsAction() {
                ITestObject obj3 = NewTestObject<Object3>();
                obj3.GetAction("Auto Complete Prop1");
                obj3.GetAction("Auto Complete Prop2");
                obj3.GetAction("Auto Complete Prop3");
                obj3.GetAction("Auto Complete 0 Do Somthing");
                obj3.GetAction("Auto Complete 0 Do Something");
            }
        }

        public class Object1 {
            public IDomainObjectContainer Container { set; protected get; }

            public string Prop1 { get; set; }

            public string Prop2 { get; set; }

            public Object2 Prop3 { get; set; }

            public IQueryable<int> AutoCompleteProp1(string autoCompleteParm) {
                return new List<int> {4, 8, 9}.AsQueryable();
            }

            public IQueryable<string> AutoCompleteProp2(string autoCompleteParm) {
                return new List<string> {"Fee", "Foo", "Fuu"}.AsQueryable();
            }

            public IQueryable<Object2> AutoCompleteProp3(string autoCompleteParm) {
                return Container.Instances<Object2>();
            }

            #region Do Something

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public IQueryable<int> AutoComplete0DoSomething(string autoCompleteParm) {
                return AutoCompleteProp1(autoCompleteParm);
            }

            public IQueryable<string> AutoComplete1DoSomething(string autoCompleteParm) {
                return AutoCompleteProp2(autoCompleteParm);
            }

            public IQueryable<Object2> AutoComplete2DoSomething(string autoCompleteParm) {
                return AutoCompleteProp3(autoCompleteParm);
            }

            #endregion
        }

        public class Object2 {
            [Title]
            public string Prop1 { get; set; }
        }

        public class Object3 {
            public int Prop1 { get; set; }

            public string Prop2 { get; set; }

            public IQueryable<string> AutoCompleteProp1() {
                return null;
            }

            public string AutoCompleteProp2(string autoCompleteParm) {
                return null;
            }

            public string AutoCompleteProp3(string autoCompleteParm) {
                return null;
            }

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public IQueryable<int> AutoComplete0DoSomthing(string autoCompleteParm) {
                return null;
            }

            public IQueryable<string> AutoComplete0DoSomething(string autoCompleteParm) {
                return null;
            }
        }
    }
}