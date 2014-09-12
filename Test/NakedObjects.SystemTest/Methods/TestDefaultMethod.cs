// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.ComponentModel;
using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace Default {
        [TestClass]
        public class TestDefaultMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
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

            [TestCleanup()]
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
            public void DefaultMethodDoesNotShowUpAsAction() {
                ITestObject obj = NewTestObject<Object1>();
                try {
                    obj.GetAction("Default Prop1");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }

                try {
                    obj.GetAction("Default 0 Do Something");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public virtual void DefaultNumericProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop1");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("8", def);
            }

            [TestMethod]
            public void DefaultParameters() {
                ITestAction action = obj1.GetAction("Do Something");
                string def0 = action.Parameters[0].GetDefault().Title;
                Assert.IsNotNull(def0);
                Assert.AreEqual("8", def0);

                string def1 = action.Parameters[1].GetDefault().Title;
                Assert.IsNotNull(def1);
                Assert.AreEqual("Foo", def1);

                string def2 = action.Parameters[2].GetDefault().Title;
                Assert.IsNotNull(def2);
                Assert.AreEqual("Bar2", def2);
            }

            [TestMethod]
            public virtual void DefaultReferenceProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop3");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("Bar2", def);
            }

            [TestMethod]
            public virtual void DefaultStringProperty() {
                ITestProperty prop = obj1.GetPropertyByName("Prop2");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("Foo", def);
            }

            [TestMethod]
            public void UnmatchedDefaultMethodShowsUpAsAction() {
                ITestObject obj = NewTestObject<Object3>();
                obj.GetAction("Default Prop1");
                obj.GetAction("Default Prop2");
                obj.GetAction("Default 0 Do Somthing");
                obj.GetAction("Default 0 Do Something");
            }

            [TestMethod]
            public void DefaultNumericMethodOverAnnotation() {

                ITestProperty prop = obj1.GetPropertyByName("Prop4");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("8", def);
            }

            [TestMethod]
            public void DefaultStringMethodOverAnnotation() {
                ITestProperty prop = obj1.GetPropertyByName("Prop5");
                string def = prop.GetDefault().Title;
                Assert.IsNotNull(def);
                Assert.AreEqual("Foo", def);
            }

            [TestMethod]
            public void DefaultParametersOverAnnotation() {
                ITestAction action = obj1.GetAction("Do Something Else");
                string def0 = action.Parameters[0].GetDefault().Title;
                Assert.IsNotNull(def0);
                Assert.AreEqual("8", def0);

                string def1 = action.Parameters[1].GetDefault().Title;
                Assert.IsNotNull(def1);
                Assert.AreEqual("Foo", def1);
            }
        }

        public class Object1 {
            public IDomainObjectContainer Container { set; protected get; }

            public int Prop1 { get; set; }

            public string Prop2 { get; set; }

            public Object2 Prop3 { get; set; }

            public int DefaultProp1() {
                return 8;
            }

            public string DefaultProp2() {
                return "Foo";
            }

            public Object2 DefaultProp3() {
                return Container.Instances<Object2>().Where(x => x.Prop1 == "Bar2").FirstOrDefault();
            }

            [DefaultValue(7)]
            public int Prop4 { get; set; }

            [DefaultValue("Bar")]
            public string Prop5 { get; set; }

         
            public int DefaultProp4() {
                return 8;
            }

            public string DefaultProp5() {
                return "Foo";
            }

            #region Do Something

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public int Default0DoSomething() {
                return DefaultProp1();
            }

            public string Default1DoSomething() {
                return DefaultProp2();
            }

            public Object2 Default2DoSomething() {
                return DefaultProp3();
            }

            #endregion

            #region Do Something Else

            public void DoSomethingElse([DefaultValue(7)] int param0,
                                        [DefaultValue("Bar")] string param1) { }

            public int Default0DoSomethingElse() {
                return DefaultProp1();
            }

            public string Default1DoSomethingElse() {
                return DefaultProp2();
            }

           

            #endregion

        }

        public class Object2 {
            [Title]
            public string Prop1 { get; set; }
        }

        public class Object3 {
            public string Prop1 { get; set; }

            public int DefaultProp1() {
                return 0;
            }

            public string DefaultProp2() {
                return null;
            }

            public void DoSomething(int param0, string param1, Object2 param2) {}

            public string Default0DoSomthing(int param0) {
                return null;
            }

            public string Default0DoSomething(decimal param0) {
                return null;
            }
        }
    }
}