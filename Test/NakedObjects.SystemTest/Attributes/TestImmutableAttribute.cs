// Copyright © Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using System;
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NakedObjects.SystemTest.Attributes {
    namespace Immutable {
        [TestClass]
        public class TestImmutableAttribute : AbstractSystemTest {
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
                                                                  new SimpleRepository<Object1>(),
                                                                  new SimpleRepository<Object2>(),
                                                                  new SimpleRepository<Object3>()
                                                              });
                }
            }
#endregion

            [TestMethod]
            public virtual void ObjectImmutable() {
                ITestObject obj = NewTestObject<Object2>();
                ITestProperty prop0 = obj.GetPropertyByName("Prop0");
                prop0.AssertIsUnmodifiable();
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.AssertIsUnmodifiable();
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertIsUnmodifiable();
                ITestProperty prop3 = obj.GetPropertyByName("Prop3");
                prop3.AssertIsUnmodifiable();
                ITestProperty prop4 = obj.GetPropertyByName("Prop4");
                prop4.AssertIsUnmodifiable();
                ITestProperty prop5 = obj.GetPropertyByName("Prop5");
                prop5.AssertIsUnmodifiable();
                ITestProperty prop6 = obj.GetPropertyByName("Prop6");
                prop6.AssertIsUnmodifiable();
                obj.Save();
                prop0.AssertIsUnmodifiable();
                prop1.AssertIsUnmodifiable();
                prop2.AssertIsUnmodifiable();
                prop3.AssertIsUnmodifiable();
                prop4.AssertIsUnmodifiable();
                prop5.AssertIsUnmodifiable();
                prop6.AssertIsUnmodifiable();
            }

            [TestMethod]
            public virtual void TestObjectImmutableOncePersisted() {
                ITestObject obj = NewTestObject<Object3>();
                ITestProperty prop0 = obj.GetPropertyByName("Prop0");
                prop0.AssertIsModifiable();
                ITestProperty prop1 = obj.GetPropertyByName("Prop1");
                prop1.AssertIsUnmodifiable();
                ITestProperty prop2 = obj.GetPropertyByName("Prop2");
                prop2.AssertIsModifiable();
                ITestProperty prop3 = obj.GetPropertyByName("Prop3");
                prop3.AssertIsUnmodifiable();
                ITestProperty prop4 = obj.GetPropertyByName("Prop4");
                prop4.AssertIsModifiable();
                ITestProperty prop5 = obj.GetPropertyByName("Prop5");
                prop5.AssertIsUnmodifiable();
                ITestProperty prop6 = obj.GetPropertyByName("Prop6");
                prop6.AssertIsModifiable();

                obj.Save();

                prop0.AssertIsUnmodifiable();
                prop1.AssertIsUnmodifiable();
                prop2.AssertIsUnmodifiable();
                prop3.AssertIsUnmodifiable();
                prop4.AssertIsUnmodifiable();
                prop5.AssertIsUnmodifiable();
                prop6.AssertIsUnmodifiable();
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

        [Immutable]
        public class Object2 : Object1 {
            public void ChangeProp1() {
                Prop1 = "Foo";
            }
        }

        [Immutable(WhenTo.OncePersisted)]
        public class Object3 : Object1 {}
    }
} //end of root namespace