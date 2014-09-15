// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System.Linq;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Title {
        [TestClass, Ignore]
        public class TestTitleAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetUp() {
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
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>(),
                                                                  new SimpleRepository<Object3>(),
                                                                  new SimpleRepository<Object4>(),
                                                                  new SimpleRepository<Object5>(),
                                                                  new SimpleRepository<Object6>(),
                                                                  new SimpleRepository<Object7>(),
                                                                  new SimpleRepository<Object8>(),
                                                                  new SimpleRepository<Object9>(),
                                                              });
                }
            }
            #endregion

            private ITestService rep;
            private ITestObject obj;
            private ITestProperty prop1;

           

     

            [TestMethod]
            public virtual void ObjectWithTitleAttributeOnString() {
                rep = GetTestService("Object1s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                obj.AssertTitleEquals("Untitled Object1");
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Foo");
                obj.AssertTitleEquals("Foo");
                obj.Save();
                obj.AssertTitleEquals("Foo");
            }

            [TestMethod]
            public virtual void TitleAttributeOnReferencePropertyThatHasATitleAttribute() {
                ITestService rep1 = GetTestService("Object1s");
                ITestObject obj1 = rep1.GetAction("New Instance").InvokeReturnObject();
                obj1.GetPropertyByName("Prop1").SetValue("Foo");
                obj1.AssertTitleEquals("Foo");
                obj1.Save();

                ITestService rep8 = GetTestService("Object8s");
                ITestObject obj8 = rep8.GetAction("New Instance").InvokeReturnObject();
                obj8.AssertTitleEquals("Untitled Object8");
                prop1 = obj8.GetPropertyByName("Prop1");
                prop1.SetObject(obj1);
                obj8.AssertTitleEquals("Foo");
            }

            [TestMethod]
            public virtual void TitleAttributeOnReferencePropertyThatHasATitleMethod() {
                ITestService rep4 = GetTestService("Object4s");
                ITestObject obj4 = rep4.GetAction("New Instance").InvokeReturnObject();
                obj4.GetPropertyByName("Prop1").SetValue("Foo");
                obj4.AssertTitleEquals("Foo");
                obj4.Save();

                ITestService rep7 = GetTestService("Object7s");
                ITestObject obj7 = rep7.GetAction("New Instance").InvokeReturnObject();
                obj7.AssertTitleEquals("Untitled Object7");
                prop1 = obj7.GetPropertyByName("Prop1");
                prop1.SetObject(obj4);
                obj7.AssertTitleEquals("Foo");
            }

            [TestMethod]
            public virtual void TitleAttributeOnReferencePropertyThatHasAToString() {
                ITestService rep2 = GetTestService("Object2s");
                ITestObject obj2 = rep2.GetAction("New Instance").InvokeReturnObject();
                obj2.GetPropertyByName("Prop1").SetValue("Foo");
                var dom2 = (Object2) obj2.GetDomainObject();
                StringAssert.Equals("Foo", dom2.ToString());
                obj2.AssertTitleEquals("Foo");
                obj2.Save();

                ITestService rep9 = GetTestService("Object9s");
                ITestObject obj9 = rep9.GetAction("New Instance").InvokeReturnObject();
                obj9.AssertTitleEquals("Untitled Object9");
                prop1 = obj9.GetPropertyByName("Prop1");
                prop1.SetObject(obj2);
                obj9.AssertTitleEquals("Foo");
            }

            [TestMethod]
            public virtual void TitleAttributeTakesPrecedenceOverTitleMethod() {
                rep = GetTestService("Object6s");
                obj = rep.GetAction("New Instance").InvokeReturnObject();
                var dom = (Object6) obj.GetDomainObject();
                StringAssert.Equals("Bar", dom.ToString());
                StringAssert.Equals("Hex", dom.Title());
                obj.AssertTitleEquals("Untitled Object6");
                prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Foo");
                obj.AssertTitleEquals("Foo");
                obj.Save();
                obj.AssertTitleEquals("Foo");

           
            }

            [TestMethod]
            public virtual void TitleAttributeTakesPrecedenceOverToString() {
                rep = GetTestService("Object3s");
                ITestObject obj = rep.GetAction("New Instance").InvokeReturnObject();
                StringAssert.Equals("Bar", obj.GetDomainObject().ToString());
                obj.AssertTitleEquals("Untitled Object3");
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.SetValue("Foo");
                obj.AssertTitleEquals("Foo");
                obj.Save();
                obj.AssertTitleEquals("Foo");

              
            }
        }

        public class Object1 {
            [Title]
            [Optionally]
            public string Prop1 { get; set; }
        }

        public class Object2 {
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

        public class Object6 {
            [Title]
            public string Prop1 { get; set; }

            public string Title() {
                return "Hex";
            }

            public override string ToString() {
                return "Bar";
            }
        }

        public class Object7 {
            [Title]
            public Object4 Prop1 { get; set; }
        }

        public class Object8 {
            [Title]
            public Object1 Prop1 { get; set; }
        }

        public class Object9 {
            [Title]
            public Object2 Prop1 { get; set; }
        }
    }
} //end of root namespace