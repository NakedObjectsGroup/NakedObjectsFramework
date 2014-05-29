// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace Choices {
        [TestClass]
        public class TestChoicesMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
                InitializeNakedObjectsFramework();
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

            [TestCleanup()]
            public void TearDown() {
                CleanupNakedObjectsFramework();
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
            public void ChoicesMethodDoesNotShowUpAsAction() {
                try {
                    obj1.GetAction("Choices Prop1");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }

                try {
                    obj1.GetAction("Choices 0 Do Something");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public virtual void ChoicesNumericProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop1");
                ITestNaked[] cho = prop.GetChoices();
                Assert.AreEqual(3, cho.Count());
                Assert.AreEqual("4", cho[0].Title);
            }

            [TestMethod]
            public virtual void ChoicesParameters() {
                ITestAction action = obj1.GetAction("Do Something");
                ITestNaked[] cho0 = action.Parameters[0].GetChoices();
                Assert.AreEqual(3, cho0.Count());
                Assert.AreEqual("4", cho0[0].Title);

                ITestNaked[] cho1 = action.Parameters[1].GetChoices();
                Assert.AreEqual(3, cho1.Count());
                Assert.AreEqual("Fee", cho1[0].Title);

                ITestNaked[] cho2 = action.Parameters[2].GetChoices();
                Assert.AreEqual(3, cho2.Count());
                Assert.AreEqual("Bar1", cho2[0].Title);
            }

            [TestMethod]
            public virtual void ChoicesReferenceProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop3");
                ITestNaked[] cho = prop.GetChoices();
                Assert.AreEqual(3, cho.Count());
                Assert.AreEqual("Bar1", cho[0].Title);
            }

            [TestMethod]
            public virtual void ChoicesStringProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop2");
                ITestNaked[] cho = prop.GetChoices();
                Assert.AreEqual(3, cho.Count());
                Assert.AreEqual("Fee", cho[0].Title);
            }

            [TestMethod]
            public void UnmatchedChoicesMethodShowsUpAsAction() {
                ITestObject obj3 = NewTestObject<Object3>();
                obj3.GetAction("Choices Prop1");
                obj3.GetAction("Choices Prop2");
                obj3.GetAction("Choices Prop3");
                obj3.GetAction("Choices 0 Do Somthing");
                obj3.GetAction("Choices 0 Do Something");
            }
        }

        public class Object1 {
            public IDomainObjectContainer Container { set; protected get; }

            public int Prop1 { get; set; }

            public string Prop2 { get; set; }

            public Object2 Prop3 { get; set; }

            public List<int> ChoicesProp1() {
                return new List<int> {4, 8, 9};
            }

            public List<string> ChoicesProp2() {
                return new List<string> {"Fee", "Foo", "Fuu"};
            }

            public List<Object2> ChoicesProp3() {
                return Container.Instances<Object2>().ToList();
            }

            #region Do Something

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public List<int> Choices0DoSomething() {
                return ChoicesProp1();
            }

            public List<string> Choices1DoSomething() {
                return ChoicesProp2();
            }

            public List<Object2> Choices2DoSomething() {
                return ChoicesProp3();
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

            public List<string> ChoicesProp1() {
                return null;
            }

            public string ChoicesProp2() {
                return null;
            }

            public string ChoicesProp3() {
                return null;
            }

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public List<int> Choices0DoSomthing() {
                return null;
            }

            public List<string> Choices0DoSomething() {
                return null;
            }
        }
    }
}