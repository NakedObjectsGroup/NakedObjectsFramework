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
    namespace DisableDefault {
        [TestClass, Ignore]
        public class TestDisableDefault : AbstractSystemTest {
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
            public void DisableActionDefault() {
                ITestObject obj = NewTestObject<Object2>();
                obj.GetAction("Action1").AssertIsDisabled();
                obj.GetAction("Action2").AssertIsDisabled();
            }

            [TestMethod]
            public void DisableActionDefaultDoesNotDisableProperties() {
                ITestObject obj = NewTestObject<Object2>();
                obj.GetPropertyByName("Prop1").AssertIsModifiable();
                obj.GetPropertyByName("Prop2").AssertIsModifiable();
                //obj.GetPropertyByName("Prop4").AssertIsModifiable(); - collection disabled by default
            }


            [TestMethod]
            public void DisableActionDefaultDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object2>();
                try {
                    obj.GetAction("Disable Action Default");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                    //TODO:  Test exception message (see #804)
                }
            }

            [TestMethod]
            public void DisableActionDefaultOverriddenByActionLevelMethod() {
                ITestObject obj = NewTestObject<Object2>();
                obj.GetAction("Action3").AssertIsEnabled();
            }

            [TestMethod]
            public void DisablePropertyDefault() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetPropertyByName("Prop1").AssertIsUnmodifiable();
                obj.GetPropertyByName("Prop2").AssertIsUnmodifiable();
                obj.GetPropertyByName("Prop4").AssertIsUnmodifiable();
            }

            [TestMethod]
            public void DisablePropertyDefaultDoesNotDisableActions() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetAction("Action1").AssertIsEnabled();
                obj.GetAction("Action2").AssertIsEnabled();
            }

            [TestMethod]
            public void DisablePropertyDefaultDoesNotShowUpAsAnAction() {
                ITestObject obj = NewTestObject<Object1>();
                try {
                    obj.GetAction("Disable Property Default ");
                    Assert.Fail();
                }
                catch (Exception e) {
                    Assert.IsNotNull(e);
                }
            }

            [TestMethod]
            public void DisablePropertyPropertyOverriddenByPropertyLevelMethod() {
                ITestObject obj = NewTestObject<Object1>();
                obj.GetPropertyByName("Prop3").AssertIsModifiable();
                //obj.GetPropertyByName("Prop5").AssertIsModifiable(); - collection disabled by default
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

            public string DisablePropertyDefault() {
                return "This property has been disabled by default";
            }

            public string DisableProp3() {
                return null;
            }

            public string DisableProp5() {
                return null;
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

            public string DisableActionDefault() {
                return "This property has been disabled by default";
            }

            public void Action1() {}

            public void Action2(string param) {}

            public void Action3() {}

            public string DisableAction3() {
                return null;
            }
        }
    }
}

//end of root namespace