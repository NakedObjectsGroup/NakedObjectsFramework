// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Disabled {
        [TestClass, Ignore]
        public class TestDisabledAttribute : AbstractSystemTest {
            #region Setup/Teardown

            [TestInitialize()]
            public void SetupTest() {
                InitializeNakedObjectsFramework(this);
                
            }

            [TestCleanup()]
            public void TearDownTest() {
                CleanupNakedObjectsFramework(this);
                
            }

            #endregion

            //public TestDisabledAttribute(string name) : base(name) {}

            //public TestDisabledAttribute()
            //    : this(typeof (TestDisabledAttribute).Name) {}

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

            [TestMethod]
            public virtual void Disabled() {
                ITestObject obj = GetTestService("Object1s").GetAction("New Instance").InvokeReturnObject();
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.AssertIsUnmodifiable();

                obj.Save();
                prop1.AssertIsUnmodifiable();
            }

            [TestMethod]
            public virtual void DisabledAlways() {
                ITestObject obj = GetTestService("Object1s").GetAction("New Instance").InvokeReturnObject();
                ITestProperty prop5 = obj.GetPropertyByName("Prop5");
                prop5.AssertIsUnmodifiable();
                obj.Save();
                prop5.AssertIsUnmodifiable();
            }

            [TestMethod]
            public virtual void DisabledNever() {
                ITestObject obj = GetTestService("Object1s").GetAction("New Instance").InvokeReturnObject();
                ITestProperty prop4 = obj.GetPropertyByName("Prop4");
                prop4.AssertIsModifiable();
                obj.Save();
                prop4.AssertIsModifiable();
            }

            [TestMethod]
            public virtual void DisabledOncePersisted() {
                ITestObject obj = GetTestService("Object1s").GetAction("New Instance").InvokeReturnObject();
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertIsModifiable();
                obj.Save();
                prop2.AssertIsUnmodifiable();
            }

            [TestMethod]
            public virtual void DisabledUntilPersisted() {
                ITestObject obj = GetTestService("Object1s").GetAction("New Instance").InvokeReturnObject();
                ITestProperty prop3 = obj.GetPropertyByName("Prop3");
                prop3.AssertIsUnmodifiable();
                obj.Save();
                prop3.AssertIsModifiable();
            }
        }

        public class Object1 {
            public Object1() {
                // initialise all fields 
                Prop0 = Prop1 = Prop2 = Prop3 = Prop4 = Prop5 = Prop6 = string.Empty;
            }

            public string Prop0 { get; set; }

            [Disabled]
            public string Prop1 { get; set; }

            [Disabled(WhenTo.OncePersisted)]
            public string Prop2 { get; set; }

            [Disabled(WhenTo.UntilPersisted)]
            public string Prop3 { get; set; }

            [Disabled(WhenTo.Never)]
            public string Prop4 { get; set; }

            [Disabled(WhenTo.Always)]
            public string Prop5 { get; set; }

            public string Prop6 { get; set; }

            public string DisableProp6() {
                if (Prop4 == "Disable 6") {
                    return "Disabled Message";
                }
                return null;
            }
        }
    }
} //end of root namespace