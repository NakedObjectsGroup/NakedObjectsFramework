// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace Title {
        [TestClass]
        public class TestTitlesMethod : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework();
                base.StartMethodProfiling();
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework();
                base.StopMethodProfiling();
                rep = null;
                results = null;
                obj = null;
                prop1 = null;
            }

            #endregion

            #region "Services & Fixtures"
            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object2>(),
                                                                  new SimpleRepository<Object3>(),
                                                                  new SimpleRepository<Object4>(),
                                                                  new SimpleRepository<Object5>(),
                                                              });
                }
            }
            #endregion

            private ITestService rep;
            private ITestCollection results;
            private ITestObject obj;
            private ITestProperty prop1;

            [TestMethod]
            public virtual void FindByTitleDoesNotWorkOnToString() {
                rep = GetTestService("Object2s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                obj.Save();
                ITestAction findByTitle = rep.GetAction("Find By Title");

                results = findByTitle.InvokeReturnCollection("F");
                results.AssertCountIs(0);

                obj.GetPropertyByName("Prop1").SetValue("Foo");

                results = findByTitle.InvokeReturnCollection("F");
                results.AssertCountIs(0);
            }

            [TestMethod]
            public virtual void FindByTitleDoesNotWorkWithTitleMethod() {
                rep = GetTestService("Object4s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                prop1 = obj.GetPropertyByName("Prop1");
                obj.Save();

                ITestAction findByTitle = rep.GetAction("Find By Title");
                results = findByTitle.InvokeReturnCollection("F");
                results.AssertCountIs(0);
            }

            [TestMethod]
            public virtual void ObjectWithSimpleToString() {
                rep = GetTestService("Object2s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Bar");
                obj.AssertTitleEquals("Bar");
                obj.Save();
                obj.AssertTitleEquals("Bar");
            }

            [TestMethod]
            public virtual void TitleMethod() {
                rep = GetTestService("Object4s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                obj.AssertTitleEquals("Untitled Object4");
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Foo");
                obj.AssertTitleEquals("Foo");
                obj.Save();
                obj.AssertTitleEquals("Foo");
            }

            [TestMethod]
            public virtual void TitleMethodTakesPrecedenceOverToString() {
                rep = GetTestService("Object5s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                StringAssert.Equals("Bar", obj.GetDomainObject().ToString());
                obj.AssertTitleEquals("Untitled Object5");
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Foo");
                obj.AssertTitleEquals("Foo");
                obj.Save();
                obj.AssertTitleEquals("Foo");
            }
        }

        public class Object2 {
            [Optionally]
            public string Prop1 { get; set; }

            public override string ToString() {
                return Prop1;
            }
        }

        public class Object3 {
            [Title]
            public string Prop1 { get; set; }

            public override string ToString() {
                return "Bar";
            }
        }

        public class Object4 {
            [Optionally]
            public string Prop1 { get; set; }

            public string Title() {
                return Prop1;
            }
        }

        public class Object5 {
            public string Prop1 { get; set; }

            public string Title() {
                return Prop1;
            }

            public override string ToString() {
                return "Bar";
            }
        }
    }
} //end of root namespace