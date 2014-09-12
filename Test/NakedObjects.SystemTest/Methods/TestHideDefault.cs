// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using System.Collections.Generic;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Methods {
    namespace HideDefault {
        [TestClass]
        public class TestHideDefault : AbstractSystemTest {
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

            protected override IFixturesInstaller Fixtures {
                get { return new FixturesInstaller(new object[] {}); }
            }

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>()
                                                              });
                }
            }

            [TestMethod]
            public void HideActionDefault() {
                ITestObject obj = NewTestObject<Object2>();
                obj.GetAction("Action1").AssertIsInvisible();
                obj.GetAction("Action2").AssertIsInvisible();
            }

            [TestMethod]
            public void HideActionDefaultDoesNotHideProperties() {
                ITestObject obj = NewTestObject<Object2>();
                obj.GetPropertyByName("Prop1").AssertIsVisible();
                obj.GetPropertyByName("Prop2").AssertIsVisible();
                obj.GetPropertyByName("Prop4").AssertIsVisible();
            }


            [TestMethod]
            public void HideActionDefaultDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object2>();
                try {
                    obj.GetAction("Hide Action Default");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                    //TODO:  Test exception message (see #804)
                }
            }

            [TestMethod]
            public void HideActionDefaultOverriddenByActionLevelMethod() {
                ITestObject obj = NewTestObject<Object2>();
                obj.GetAction("Action3").AssertIsVisible();
            }

            [TestMethod]
            public void HidePropertyDefault() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetPropertyByName("Prop1").AssertIsInvisible();
                obj.GetPropertyByName("Prop2").AssertIsInvisible();
                obj.GetPropertyByName("Prop4").AssertIsInvisible();
            }

            [TestMethod]
            public void HidePropertyDefaultDoesNotHideActions() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetAction("Action1").AssertIsVisible();
                obj.GetAction("Action2").AssertIsVisible();
            }

            [TestMethod]
            public void HidePropertyDefaultDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object1>();
                try {
                    obj.GetAction("Hide Property Default ");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public void HidePropertyPropertyOverriddenByPropertyLevelMethod() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetPropertyByName("Prop3").AssertIsVisible();
                obj.GetPropertyByName("Prop5").AssertIsVisible();
            }
        }

        public class Object1 {
            private ICollection<Object1> prop4 = new List<Object1>();
            private ICollection<Object1> prop5 = new List<Object1>();
            public string Prop1 { get; set; }

            public Object1 Prop2 { get; set; }


            public string Prop3 { get; set; }

            public ICollection<Object1> Prop4 {
                get { return prop4; }
                set { prop4 = value; }
            }

            public ICollection<Object1> Prop5 {
                get { return prop5; }
                set { prop5 = value; }
            }

            public bool HidePropertyDefault() {
                return true;
            }

            public bool HideProp3() {
                return false;
            }

            public bool HideProp5() {
                return false;
            }

            public void Action1() {}

            public void Action2(string param) {}
        }


        public class Object2 {
            private ICollection<Object1> prop4 = new List<Object1>();
            public string Prop1 { get; set; }

            public Object1 Prop2 { get; set; }

            public ICollection<Object1> Prop4 {
                get { return prop4; }
                set { prop4 = value; }
            }

            public bool HideActionDefault() {
                return true;
            }


            public void Action1() {}

            public void Action2(string param) {}

            public void Action3() {}

            public bool HideAction3() {
                return false;
            }
        }
    }
}

//end of root namespace