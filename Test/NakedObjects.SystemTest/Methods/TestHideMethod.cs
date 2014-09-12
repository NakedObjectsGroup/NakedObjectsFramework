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
    namespace Hide {
        [TestClass]
        public class TestHideMethod : AbstractSystemTest {
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

            [TestMethod]
            public void HideAction() {
                ITestObject obj = NewTestObject<Object1>();
                obj.Save();
                obj.GetAction("Do Something");
                obj.GetPropertyByName("Prop4").SetValue("Hide 6");

                try {
                    obj.GetAction("Do Something");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                    //TODO:  Test exception message (see #804)
                }
            }

            [TestMethod]
            public void HideMethodDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object1>();
                try {
                    obj.GetAction("Hide Prop6");
                    Assert.Fail("'Hide Prop6' is showing as an action");
                }
                catch (AssertFailedException e) {
                    Assert.AreEqual("Assert.Fail failed. No Action named 'Hide Prop6'", e.Message);
                }
            }

            [TestMethod]
            public void HideProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop6 = obj.GetPropertyByName("Prop6");
                prop6.AssertIsVisible();
                obj.Save();
                prop6.AssertIsVisible();

                ITestProperty prop4 = obj.GetPropertyByName("Prop4");
                prop4.SetValue("Hide 6");
                prop6.AssertIsInvisible();
            }

            [TestMethod]
            public void UnmatchedHideMethodShowsUpAsAnAction() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetAction("Hide Prop7");
                obj.GetAction("Hide Prop4");
                obj.GetAction("Hide Do Something Else");
                obj.GetAction("Hide Do Somthing Else");
            }
        }

        public class Object1 {
            [Optionally]
            public string Prop4 { get; set; }

            [Optionally]
            public string Prop6 { get; set; }

            public bool HideProp6() {
                return Prop4 == "Hide 6";
            }

            public void DoSomething() {}

            public bool HideDoSomething() {
                return HideProp6();
            }

            public void DoSomethingElse() {}

            public bool HideProp7() {
                return false;
            }

            public string HideProp4() {
                return null;
            }

            public bool HideDoSomthingElse() {
                return false;
            }

            public string HideDoSomethingElse() {
                return null;
            }
        }
    }
}