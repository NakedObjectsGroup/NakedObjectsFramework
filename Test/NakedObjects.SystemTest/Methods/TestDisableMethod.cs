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
    namespace Disable {
        [TestClass]
        public class TestDisableMethod : AbstractSystemTest {
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
            public void DisableAction() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetPropertyByName("Prop4").SetValue("avalue");
                obj.GetPropertyByName("Prop6").SetValue("avalue");
                obj.Save();
                obj.GetAction("Do Something").AssertIsEnabled();

                obj.GetPropertyByName("Prop4").SetValue("Disable 6");
                obj.GetAction("Do Something").AssertIsDisabled();
            }

            [TestMethod]
            public void DisableMethodDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object1>();
                try {
                    obj.GetAction("Disable Prop6");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                    //TODO:  Test exception message (see #804)
                }
            }

            [TestMethod]
            public void DisableProperty() {
                ITestObject obj = NewTestObject<Object1>();
                ITestProperty prop6 = obj.GetPropertyByName("Prop6");
                prop6.AssertIsModifiable();
                obj.Save();
                prop6.AssertIsModifiable();

                ITestProperty prop4 = obj.GetPropertyByName("Prop4");
                prop4.SetValue("Disable 6");
                prop6.AssertIsUnmodifiable();
            }

            [TestMethod]
            public void UnmatchedDisableMethodShowsUpAsAction() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetAction("Disable Prop1");
                obj.GetAction("Disable Prop4");
            }
        }

        public class Object1 {
            [Disabled(WhenTo.Never)]
            [Optionally]
            public string Prop4 { get; set; }

            [Optionally]
            public string Prop6 { get; set; }

            public string DisableProp6() {
                if (Prop4 == "Disable 6") {
                    return "Disabled Message";
                }
                return null;
            }

            public string DisableProp1() {
                return null;
            }

            public bool DisableProp4() {
                return false;
            }

            public void DoSomething() {}

            public string DisableDoSomething() {
                return DisableProp6();
            }
        }
    }
} //end of root namespace